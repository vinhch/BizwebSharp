using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BizwebSharp.Enums;
using BizwebSharp.Infrastructure;
using Microsoft.Extensions.Primitives;
using RestSharp.Portable;

namespace BizwebSharp
{
    public static class AuthorizationService
    {
        private static IList<string> QueryKeyForHash { get; } = new List<string>
        {
            "code",
            "store",
            "timestamp"
        };

        private static Func<string, bool, string> EncodeQuery { get; } = (s, isKey) =>
        {
            //Important: Replace % before replacing &. Else second replace will replace those %25s.
            var output = s?.Replace("%", "%25").Replace("&", "%26") ?? "";

            if (isKey)
                output = output.Replace("=", "%3D");

            return output;
        };

        private static string PrepareQuerystring(NameValueCollection querystring, string joinWith)
        {
            var queryDictionary = new Dictionary<string, string>();
            foreach (var item in querystring)
            {
                var key = (string)item;
                var value = querystring[key];

                if (!QueryKeyForHash.Contains(key)) continue;

                queryDictionary[EncodeQuery(key, true)] = EncodeQuery(value, false);
            }
            var kvps = queryDictionary.OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{kvp.Key}={kvp.Value}");

            return string.Join(joinWith, kvps);
        }

        private static string PrepareQuerystring(IEnumerable<KeyValuePair<string, StringValues>> querystring, string joinWith)
        {
            var queryDictionary = new Dictionary<string, string>();
            foreach (var kvp in querystring)
            {
                var key = kvp.Key;
                var value = kvp.Value;

                if (!QueryKeyForHash.Contains(key)) continue;

                queryDictionary[EncodeQuery(key, true)] = EncodeQuery(value, false);
            }
            var kvps = queryDictionary.OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{kvp.Key}={kvp.Value}");

            return string.Join(joinWith, kvps);
        }

        // To calculate HMAC signature:
        // 1. Cast querystring to KVP pairs (key-value pair).
        // 2. Remove `signature` and `hmac` keys.
        // 3. Replace & with %26, % with %25 in keys and values.
        // 4. Replace = with %3D in keys only.
        // 5. Join each key and value with = (key=value).
        // 6. Sorty kvps alphabetically.
        // 7. Join kvps together with & (key=value&key=value&key=value).
        // 8. Compute the kvps with an HMAC-SHA256 using the secret key.
        // 9. Request is authentic if the computed string equals the `hash` in query string.
        // Reference: https://docs.shopify.com/api/guides/authentication/oauth#making-authenticated-requests

        public static bool IsAuthenticRequest(NameValueCollection querystring, string apiSecretKey,
            double? requestTimestampSpan = null)
        {
            var hmac = querystring.Get("hmac");
            if (string.IsNullOrEmpty(hmac))
                return false;

            var kvps = PrepareQuerystring(querystring, "&");
            var timestampInString = querystring.Get("timestamp");

            return ValidateRequest(hmac, kvps, apiSecretKey, timestampInString,
                requestTimestampSpan);
        }

        public static bool IsAuthenticRequest(IEnumerable<KeyValuePair<string, StringValues>> querystring,
            string apiSecretKey,
            double? requestTimestampSpan = null)
        {
            var hmac = querystring.FirstOrDefault(kvp => kvp.Key.ToLower() == "hmac").Value;
            if (string.IsNullOrEmpty(hmac))
                return false;

            var kvps = PrepareQuerystring(querystring, "&");
            var timestampInString = querystring.First(s => s.Key == "timestamp").Value;

            return ValidateRequest(hmac, kvps, apiSecretKey, timestampInString,
                requestTimestampSpan);
        }

        public static bool ValidateRequest(string signature, string contentToCheck, string apiSecretKey,
            string timestampInString = null, double? requestTimestampSpan = null)
        {
            var hmacHasher = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecretKey));
            var hash = hmacHasher.ComputeHash(Encoding.UTF8.GetBytes(contentToCheck));

            //Convert bytes back to string, replacing dashes, to get the final signature.
            var calculatedSignature = Convert.ToBase64String(hash);

            //Request signature is valid if the calculated signature matches the signature of content.
            var isValidSignature = string.Equals(calculatedSignature, signature,
                StringComparison.CurrentCultureIgnoreCase);

            //Check request timestamp span if need
            var isValidTimestamp = true;
            double timestamp;
            if (!string.IsNullOrEmpty(timestampInString) &&
                requestTimestampSpan != null &&
                double.TryParse(timestampInString, out timestamp))
            {
                var currentTimestamp = DateTime.UtcNow.ToUnixTimestamp();
                isValidTimestamp = currentTimestamp - timestamp < requestTimestampSpan;
            }

            //Request is valid if the signature and timestamp are valid.
            return isValidSignature && isValidTimestamp;
        }

        public static bool IsAuthenticProxyRequest(NameValueCollection querystring, string apiSecretKey)
        {
            var signature = querystring.Get("signature");

            if (string.IsNullOrEmpty(signature))
                return false;

            // To calculate signature, order all querystring parameters by alphabetical (exclude the
            // signature itself). Then, hash it with the secret key.

            var kvps = PrepareQuerystring(querystring, string.Empty);

            return ValidateRequest(signature, kvps, apiSecretKey);
        }

        public static bool IsAuthenticProxyRequest(IEnumerable<KeyValuePair<string, StringValues>> querystring, string apiSecretKey)
        {
            var signature = querystring.FirstOrDefault(kvp => kvp.Key.ToLower() == "signature").Value;

            if (string.IsNullOrEmpty(signature))
                return false;

            // To calculate signature, order all querystring parameters by alphabetical (exclude the
            // signature itself). Then, hash it with the secret key.

            var kvps = PrepareQuerystring(querystring, string.Empty);

            return ValidateRequest(signature, kvps, apiSecretKey);
        }

        public static async Task<bool> IsAuthenticWebhookAsync(NameValueCollection requestHeaders, Stream inputStream,
            string apiSecretKey)
        {
            //Input stream may have already been read when a controller determines parameters to
            //pass to an action. Reset position to 0.
            inputStream.Position = 0;

            //We do not dispose the StreamReader because disposing it will also dispose the input stream,
            //and disposing a request's input stream can cause major headaches for the developer.
            string requestBody;
            using (var reader = new StreamReader(inputStream))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            return IsAuthenticWebhook(requestHeaders, requestBody, apiSecretKey);
        }

        public static bool IsAuthenticWebhook(NameValueCollection requestHeaders, string requestBody,
            string apiSecretKey)
        {
            var hmacHeader = requestHeaders.Get("X-Bizweb-Hmac-Sha256");

            if (string.IsNullOrEmpty(hmacHeader))
            {
                return false;
            }

            return ValidateRequest(hmacHeader, requestBody, apiSecretKey);
        }

        public static async Task<bool> IsAuthenticWebhookAsync(IEnumerable<KeyValuePair<string, StringValues>> requestHeaders,
            Stream inputStream, string apiSecretKey)
        {
            //Input stream may have already been read when a controller determines parameters to
            //pass to an action. Reset position to 0.
            inputStream.Position = 0;

            //We do not dispose the StreamReader because disposing it will also dispose the input stream,
            //and disposing a request's input stream can cause major headaches for the developer.
            string requestBody;
            using (var reader = new StreamReader(inputStream))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            return IsAuthenticWebhook(requestHeaders, requestBody, apiSecretKey);
        }

        public static bool IsAuthenticWebhook(IEnumerable<KeyValuePair<string, StringValues>> requestHeaders, string requestBody,
            string apiSecretKey)
        {
            var hmacHeader = requestHeaders.FirstOrDefault(kvp => kvp.Key == "X-Bizweb-Hmac-Sha256").Value;

            if (string.IsNullOrEmpty(hmacHeader))
            {
                return false;
            }

            return ValidateRequest(hmacHeader, requestBody, apiSecretKey);
        }

        public static Uri BuildAuthorizationUrl(IEnumerable<AuthorizationScope> scopes, string myApiUrl,
            string apiKey, string redirectUri, string state = null)
        {
            return BuildAuthorizationUrl(string.Join(",", scopes.Select(s => s.ToSerializedString())), myApiUrl,
                apiKey, redirectUri, state);
        }

        public static Uri BuildAuthorizationUrl(string scopes, string myApiUrl,
            string apiKey, string redirectUri, string state = null)
        {
            //Prepare a uri builder for the tenant URL
            var builder = new UriBuilder(RequestEngine.BuildUri(myApiUrl));

            //Build the querystring
            var qs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", apiKey),
                new KeyValuePair<string, string>("scope", scopes),
                new KeyValuePair<string, string>("redirect_uri", redirectUri)
            };

            if (string.IsNullOrEmpty(state) == false)
            {
                qs.Add(new KeyValuePair<string, string>("state", state));
            }

            builder.Path = "admin/oauth/authorize";
            builder.Query = string.Join("&", qs.Select(s => $"{s.Key}={s.Value}"));

            return builder.Uri;
        }

        public static async Task<string> AuthorizeAsync(string code, string myApiUrl, string apiKey,
            string apiSecretKey)
        {
            var client = RequestEngine.CreateClient(new BizwebAuthorizationState { ApiUrl = myApiUrl });
            var req = RequestEngine.CreateRequest("oauth/access_token", Method.POST);

            //Build request body
            req.AddJsonBody(new { client_id = apiKey, client_secret = apiSecretKey, code });

            var response = await RequestEngine.ExecuteRequestAsync(client, req, new DefaultRequestExecutionPolicy());

            return response.Value<string>("access_token");
        }
    }
}