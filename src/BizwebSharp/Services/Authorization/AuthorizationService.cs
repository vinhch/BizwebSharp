using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BizwebSharp.Enums;
using BizwebSharp.Extensions;
using BizwebSharp.Infrastructure;
using BizwebSharp.Infrastructure.RequestPolicies;
using Microsoft.Extensions.Primitives;
using RestSharp.Portable;

namespace BizwebSharp.Services.Authorization
{
    public static class AuthorizationService
    {
        private static IList<string> QueryKeyForHash { get; } = new List<string>
        {
            "code",
            "store",
            "timestamp"
        };

        private static Func<string, bool, string> ReplaceChars { get; } = (s, isKey) =>
        {
            //Important: Replace % before replacing &. Else second replace will replace those %25s.
            var output = s?.Replace("%", "%25").Replace("&", "%26") ?? "";

            if (isKey)
                output = output.Replace("=", "%3D");

            return output;
        };

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

            var kvps = querystring.Cast<string>()
                .Select(s => new {Key = ReplaceChars(s, true), Value = ReplaceChars(querystring[s], false)})
                .Where(kvp => QueryKeyForHash.Contains(kvp.Key))
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{kvp.Key}={kvp.Value}");

            return ValidateRequest(hmac, string.Join("&", kvps), apiSecretKey, querystring.Get("timestamp"),
                requestTimestampSpan);
        }

        public static bool IsAuthenticRequest(IEnumerable<KeyValuePair<string, StringValues>> querystring,
            string apiSecretKey,
            double? requestTimestampSpan = null)
        {
            var hmac = querystring.FirstOrDefault(q => q.Key.ToLower() == "hmac").Value;

            if (string.IsNullOrEmpty(hmac))
                return false;

            var kvps = querystring.Where(kvp => QueryKeyForHash.Contains(kvp.Key))
                .Select(s => new {Key = ReplaceChars(s.Key, true), Value = ReplaceChars(s.Value, false)})
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{kvp.Key}={kvp.Value}");

            return ValidateRequest(hmac, string.Join("&", kvps), apiSecretKey,
                querystring.First(s => s.Key == "timestamp").Value,
                requestTimestampSpan);
        }

        public static bool ValidateRequest(string hmac, string kvpInString, string apiSecretKey,
            string timestampInString = null, double? requestTimestampSpan = null)
        {
            var hmacHasher = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecretKey));
            var hash = hmacHasher.ComputeHash(Encoding.UTF8.GetBytes(kvpInString));

            //Convert bytes back to string, replacing dashes, to get the final signature.
            var calculatedSignature = Convert.ToBase64String(hash);

            //Request signature is valid if the calculated signature matches the signature from the querystring.
            var isValidSignature =
                string.Compare(hmac.ToUpper(), calculatedSignature.ToUpper(), StringComparison.Ordinal) == 0;

            //Check request timestamp span if need
            var isValidTimestamp = true;
            double timestamp;
            if ((requestTimestampSpan != null) && double.TryParse(timestampInString, out timestamp))
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

            Func<string, bool, string> replaceChars = (s, isKey) =>
            {
                //Important: Replace % before replacing &. Else second replace will replace those %25s.
                var output = s?.Replace("%", "%25").Replace("&", "%26") ?? "";

                if (isKey)
                    output = output.Replace("=", "%3D");

                return output;
            };

            var kvps = querystring.Cast<string>()
                .Select(s => new {Key = replaceChars(s, true), Value = replaceChars(querystring[s], false)})
                .Where(kvp => (kvp.Key != "signature") && (kvp.Key != "hmac"))
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{kvp.Key}={kvp.Value}");

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(string.Join(null, kvps)));

            //Convert bytes back to string, replacing dashes, to get the final signature.
            var calculatedSignature = BitConverter.ToString(hash).Replace("-", "");

            //Request is valid if the calculated signature matches the signature from the querystring.
            return string.Equals(calculatedSignature, signature, StringComparison.CurrentCultureIgnoreCase);
        }

        public static async Task<bool> IsAuthenticWebhook(NameValueCollection requestHeaders, Stream inputStream,
            string apiSecretKey)
        {
            //Input stream may have already been read when a controller determines parameters to
            //pass to an action. Reset position to 0.
            inputStream.Position = 0;

            //We do not dispose the StreamReader because disposing it will also dispose the input stream,
            //and disposing a request's input stream can cause major headaches for the developer.
            var requestBody = await new StreamReader(inputStream).ReadToEndAsync();

            return IsAuthenticWebhook(requestHeaders, requestBody, apiSecretKey);
        }

        public static bool IsAuthenticWebhook(NameValueCollection requestHeaders, string requestBody,
            string apiSecretKey)
        {
            var hmacHeader = requestHeaders.Get("X-Bizweb-Hmac-Sha256");

            if (string.IsNullOrEmpty(hmacHeader))
                return false;

            //Compute a hash from the apiKey and the request body
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecretKey));
            var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(requestBody)));

            //Webhook is valid if computed hash matches the header hash
            return hash == hmacHeader;
        }

        public static Uri BuildAuthorizationUrl(IEnumerable<AuthorizationScope> scopes, string myApiUrl,
            string apiKey, string redirectUri = null, string state = null)
        {
            return BuildAuthorizationUrl(string.Join(",", scopes.Select(s => s.ToSerializedString())), myApiUrl,
                apiKey, redirectUri, state);
        }

        public static Uri BuildAuthorizationUrl(string scopes, string myApiUrl,
            string apiKey, string redirectUri = null, string state = null)
        {
            //Prepare a uri builder for the tenant URL
            var builder = new UriBuilder(RequestEngine.BuildUri(myApiUrl));

            //Build the querystring
            var qs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", apiKey),
                new KeyValuePair<string, string>("scope", scopes)
            };

            if (string.IsNullOrEmpty(redirectUri) == false)
                qs.Add(new KeyValuePair<string, string>("redirect_uri", redirectUri));

            if (string.IsNullOrEmpty(state) == false)
                qs.Add(new KeyValuePair<string, string>("state", state));

            builder.Path = "admin/oauth/authorize";
            builder.Query = string.Join("&", qs.Select(s => $"{s.Key}={s.Value}"));

            return builder.Uri;
        }

        public static async Task<string> Authorize(string code, string myApiUrl, string apiKey,
            string apiSecretKey)
        {
            var client = RequestEngine.CreateClient(new BizwebAuthorizationState {ApiUrl = myApiUrl});
            var req = RequestEngine.CreateRequest("oauth/access_token", Method.POST);

            //Build request body
            req.AddJsonBody(new {client_id = apiKey, client_secret = apiSecretKey, code});

            var response = await RequestEngine.ExecuteRequestAsync(client, req, new DefaultRequestExecutionPolicy());

            return response.Value<string>("access_token");
        }
    }
}