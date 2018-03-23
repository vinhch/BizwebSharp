using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BizwebSharp.Enums;
using BizwebSharp.Infrastructure;
using Microsoft.Extensions.Primitives;

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

        private static readonly Regex _querystringRegex = new Regex(@"[?|&]([\w\.]+)=([^?|^&]+)", RegexOptions.Compiled);

        /// <remarks>
        /// Source for this method: https://stackoverflow.com/a/22046389
        /// </remarks>
        private static IDictionary<string, string> ParseRawQuerystring(string qs)
        {
            // Must use an absolute uri, else Uri.Query throws an InvalidOperationException
            var uri = new UriBuilder("http://localhost:3000")
            {
                Query = Uri.UnescapeDataString(qs)
            }.Uri;
            var match = _querystringRegex.Match(uri.PathAndQuery);
            var paramaters = new Dictionary<string, string>();
            while (match.Success)
            {
                paramaters.Add(match.Groups[1].Value, match.Groups[2].Value);
                match = match.NextMatch();
            }
            return paramaters;
        }

        private static Func<string, bool, string> EncodeQuery { get; } = (s, isKey) =>
        {
            //Important: Replace % before replacing &. Else second replace will replace those %25s.
            var output = s?.Replace("%", "%25").Replace("&", "%26") ?? "";

            if (isKey)
                output = output.Replace("=", "%3D");

            return output;
        };

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
        /// <summary>
        /// Determines if an incoming request is authentic.
        /// </summary>
        public static bool IsAuthenticRequest(IEnumerable<KeyValuePair<string, StringValues>> querystring,
            string apiSecretKey,
            double? requestTimestampSpan = null)
        {
            var hmac = querystring.FirstOrDefault(kvp => kvp.Key.ToLower() == "hmac").Value;
            if (string.IsNullOrEmpty(hmac))
                return false;

            var kvps = PrepareQuerystring(querystring, "&");
            var timestampInString = querystring.First(s => s.Key.ToLower() == "timestamp").Value;

            return ValidateRequest(hmac, kvps, apiSecretKey, timestampInString,
                requestTimestampSpan);
        }

        /// <summary>
        /// Determines if an incoming request is authentic.
        /// </summary>
        public static bool IsAuthenticRequest(IDictionary<string, string> querystring, string apiSecretKey)
        {
            var qs = querystring.Select(kvp => new KeyValuePair<string, StringValues>(kvp.Key, kvp.Value));

            return IsAuthenticRequest(qs, apiSecretKey);
        }

        /// <summary>
        /// Determines if an incoming request is authentic.
        /// </summary>
        public static bool IsAuthenticRequest(string querystring, string apiSecretKey)
        {
            return IsAuthenticRequest(ParseRawQuerystring(querystring), apiSecretKey);
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
            var hmacHeader =
                requestHeaders.FirstOrDefault(
                    kvp =>
                        string.Equals(kvp.Key, ApiConst.HEADER_KEY_HMAC_SHA256,
                            StringComparison.CurrentCultureIgnoreCase)).Value;

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
            return (await AuthorizeWithResultAsync(code, myApiUrl, apiKey, apiSecretKey)).AccessToken;
        }

        public static async Task<OAuthResult> AuthorizeWithResultAsync(string code, string myApiUrl, string apiKey,
            string apiSecretKey)
        {
            var authState = new BizwebAuthorizationState
            {
                ApiUrl = myApiUrl
            };

            //Build request body
            var content = new JsonContent(new
            {
                client_id = apiKey,
                client_secret = apiSecretKey,
                code
            });

            using (var reqMsg = RequestEngine.CreateRequest(authState, "oauth/access_token", HttpMethod.Post, content))
            {
                var response = await RequestEngine.ExecuteRequestAsync(reqMsg, new DefaultRequestExecutionPolicy());

                var accessToken = response.Value<string>("access_token");

                IEnumerable<string> scopes = null;
                var scope = response.Value<string>("scope");
                if (scope != null)
                {
                    scope = scope.Replace(" ", ","); // only for bizweb
                    scopes = scope.Split(',');
                }

                return new OAuthResult(accessToken, scopes);
            }
        }

        /// <summary>
        /// A convenience function that tries to ensure that a given URL is a valid Bizweb domain. It does this by making a HEAD request to the given domain, and returns true if the response contains an X-StoreId header.
        ///
        /// **Warning**: a domain could fake the response header, which would cause this method to return true.
        ///
        /// **Warning**: this method of validation is not officially supported by Bizweb and could break at any time.
        /// </summary>
        /// <param name="url">The URL of the shop to check.</param>
        /// <returns>A boolean indicating whether the URL is valid.</returns>
        public static async Task<bool> IsValidShopDomainAsync(string url)
        {
            var uri = RequestEngine.BuildUri(url);
            var client = RequestEngine.CurrentHttpClient;

            using (var msg = new HttpRequestMessage(System.Net.Http.HttpMethod.Head, uri))
            {
                try
                {
                    var response = await client.SendAsync(msg);

                    return response.Headers.Any(h => h.Key == "X-StoreId");
                }
                catch (HttpRequestException)
                {
                    return false;
                }
            }
        }

        #region method with NameValueCollection for .Net Framework

        /// <summary>
        /// Determines if an incoming request is authentic.
        /// </summary>
        public static bool IsAuthenticRequest(NameValueCollection querystring, string apiSecretKey,
            double? requestTimestampSpan = null)
        {
            return IsAuthenticRequest(querystring.ToPairs2(), apiSecretKey, requestTimestampSpan);
        }

        public static bool IsAuthenticProxyRequest(NameValueCollection querystring, string apiSecretKey)
        {
            return IsAuthenticProxyRequest(querystring.ToPairs2(), apiSecretKey);
        }

        public static async Task<bool> IsAuthenticWebhookAsync(NameValueCollection requestHeaders, Stream inputStream,
            string apiSecretKey)
        {
            return await IsAuthenticWebhookAsync(requestHeaders.ToPairs2(), inputStream, apiSecretKey);
        }

        public static bool IsAuthenticWebhook(NameValueCollection requestHeaders, string requestBody,
            string apiSecretKey)
        {
            return IsAuthenticWebhook(requestHeaders.ToPairs2(), requestBody, apiSecretKey);
        }
        #endregion
    }
}