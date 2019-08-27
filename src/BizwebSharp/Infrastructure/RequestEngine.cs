using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BizwebSharp.Infrastructure
{
    /// <summary>
    /// The Request engine - to ingest requests.
    /// </summary>
    public static class RequestEngine
    {
        public static string CreateUriPathAndQuery(string path,
            IEnumerable<KeyValuePair<string, object>> queryParams)
        {
            if (queryParams == null || !queryParams.Any())
            {
                return path;
            }
            var query = queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value.ToString())}");
            return $"{path}?{string.Join("&", query)}";
        }

        /// <summary>
        /// Attempts to build a shop API <see cref="Uri"/> for the given shop.
        /// Will throw a <see cref="BizwebSharpException"/> if the URL cannot be formatted.
        /// </summary>
        /// <param name="myApiUrl">The shop's *.bizwebvietnam.net URL.</param>
        /// <exception cref="BizwebSharpException">Thrown if the given URL cannot be converted into a well-formed URI.</exception>
        /// <returns>The shop's API <see cref="Uri"/>.</returns>
        public static Uri BuildUri(string myApiUrl, bool usingHttps = true, bool withAdminPath = true)
        {
            //var protocolScheme = usingHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
            var protocolScheme = usingHttps ? "https" : "http";
            const string schemeDelimiter = @"://"; //Uri.SchemeDelimiter

            if (Uri.IsWellFormedUriString(myApiUrl, UriKind.Absolute) == false)
            {
                if (Uri.IsWellFormedUriString(protocolScheme + schemeDelimiter + myApiUrl, UriKind.Absolute) ==
                    false)
                {
                    throw new BizwebSharpException(
                        $"The given {nameof(myApiUrl)} cannot be converted into a well-formed URI.");
                }

                myApiUrl = protocolScheme + schemeDelimiter + myApiUrl;
            }

            var builder = new UriBuilder(myApiUrl)
            {
                Path = withAdminPath ? "admin/" : "/",
                Scheme = protocolScheme,
                Port = usingHttps ? 443 : 80 //default SSL port
            };

            return builder.Uri;
        }

        /// <summary>
        /// Creates an <see cref="BizwebRequestMessage"/> by setting the method and the necessary authenticate information.
        /// </summary>
        /// <param name="authState">The Bizweb authenticate state.</param>
        /// <param name="pathAndQuery">The request's path.</param>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="content">The <see cref="HttpContent"/> to use for the request.</param>
        /// <param name="rootElement">The root element to deserialize. Default is null.</param>
        /// <returns>The prepared <see cref="BizwebRequestMessage"/>.</returns>
        public static BizwebRequestMessage CreateRequest(BizwebAuthorizationState authState,
            string pathAndQuery, HttpMethod method, HttpContent content = null, string rootElement = null)
        {
            var baseUri = BuildUri(authState.ApiUrl);
            var endPointUri = new Uri(baseUri, pathAndQuery);
            var msg = new BizwebRequestMessage(endPointUri, method, content, rootElement);

            if (!string.IsNullOrEmpty(authState.AccessToken))
            {
                msg.Headers.Add(ApiConst.HEADER_KEY_ACCESS_TOKEN, authState.AccessToken);
                msg.Headers.Add("Cache-Control", "no-cache");
            }

            msg.Headers.Add("Accept", "application/json");

            return msg;
        }

        /// <summary>
        /// Checks a response for exceptions or invalid status codes. Throws an exception when necessary.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="requestInfo">An simple request info.</param>
        /// <returns></returns>
        public static async Task CheckResponseExceptionsAsync(HttpResponseMessage response,
            RequestSimpleInfo requestInfo = null)
        {
            var statusCode = (int)response.StatusCode;
            // No error if response was between 200 and 300.
            if (statusCode >= 200 && statusCode < 300)
            {
                return;
            }

            var rawResponse = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.GetValues("Content-Type").FirstOrDefault() ?? string.Empty;
            var message = $"Response did not indicate success. Status: {statusCode} {response.ReasonPhrase}."; ;
            Dictionary<string, IEnumerable<string>> errors;

            if (contentType.StartsWithIgnoreCase("application/json") || contentType.StartsWithIgnoreCase("text/json"))
            {
                errors = ParseErrorJson(rawResponse);

                if (errors == null)
                {
                    errors = new Dictionary<string, IEnumerable<string>>
                    {
                        {
                            $"{statusCode} {response.ReasonPhrase}",
                            new[] {message}
                        }
                    };
                }
                else
                {
                    var firstError = errors.First();

                    message = $"{firstError.Key}: {string.Join(", ", firstError.Value)}";
                }
            }
            else
            {
                errors = new Dictionary<string, IEnumerable<string>>
                {
                    {
                        $"{statusCode} {response.ReasonPhrase}",
                        new[] { message }
                    },
                    {
                        "NoJsonError",
                        new[] { "Response did not return JSON, unable to parse error message (if any)." }
                    }
                };
            }

            // If the error was caused by reaching the API rate limit, throw a rate limit exception.
            if (statusCode == 429 /* Too many requests */)
            {
                var listMessage = "Exceeded 2 calls per second for api client. Reduce request rates to resume uninterrupted service.";
                var rateLimitMessage = $"Error: {listMessage}";

                // Shopify used to return JSON for rate limit exceptions, but then made an unannounced change and started returing HTML.
                // This dictionary is an attempt at preserving what was previously returned.
                errors.Add("Error", new List<string> {listMessage});

                throw new ApiRateLimitException(response.StatusCode, errors, rateLimitMessage, rawResponse, requestInfo);
            }

            throw new BizwebSharpException(response.StatusCode, errors, message, rawResponse, requestInfo);

            //if (response.ErrorException != null)
            //{
            //    //Checking this second, because Shopify errors sometimes return incomplete objects along with errors,
            //    //which cause Json deserialization to throw an exception. Parsing the Shopify error is more important
            //    //than throwing this deserialization exception.
            //    throw response.ErrorException;
            //}
        }

        private static async Task<RequestSimpleInfo> CreateRequestSimpleInfoAsync(HttpRequestMessage reqMsg)
        {
            string rawRequest = null;
            if (reqMsg.Content != null)
            {
                rawRequest = await reqMsg.Content.ReadAsStringAsync();
            }

            var requestInfo = new RequestSimpleInfo
            {
                Url = reqMsg.RequestUri.AbsoluteUri,
                Method = reqMsg.Method.ToString(),
                Body = rawRequest
            };

            if (reqMsg.Headers != null && reqMsg.Headers.Any())
            {
                requestInfo.Header = string.Join("\n", reqMsg.Headers.Select(h => $"{h.Key}: { string.Join(", ", h.Value)}"));
            }

            return requestInfo;
        }

        /// <summary>
        /// Parses a JSON string for Bizweb API errors.
        /// </summary>
        /// <returns>Returns null if the JSON could not be parsed into an error.</returns>
        private static Dictionary<string, IEnumerable<string>> ParseErrorJson(string inputStr)
        {
            if (string.IsNullOrEmpty(inputStr))
                return null;

            var errors = new Dictionary<string, IEnumerable<string>>();

            try
            {
                var parsed = JToken.Parse(string.IsNullOrEmpty(inputStr) ? "{}" : inputStr);

                // Errors can be any of the following:
                // 1. { errors: "some error message"}
                // 2. { errors: { "order" : "some error message" } }
                // 3. { errors: { "order" : [ "some error message" ] } }
                // 4. { error: "invalid_request", error_description:"The authorization code was not found or was already used" }

                if (parsed.Any(p => p.Path == "error") && parsed.Any(p => p.Path == "error_description"))
                {
                    // Error is type #4
                    var description = parsed["error_description"];

                    errors["invalid_request"] = new List<string> { description.Value<string>() };
                }
                else if (parsed.Any(x => x.Path == "errors"))
                {
                    var parsedErrors = parsed["errors"];

                    //errors can be either a single string, or an array of other errors
                    if (parsedErrors.Type == JTokenType.String)
                        errors["Error"] = new List<string> { parsedErrors.Value<string>() };
                    else
                        foreach (var val in parsedErrors.Values())
                        {
                            var name = val.Path.Split('.').Last();
                            var list = new List<string>();

                            switch (val.Type)
                            {
                                case JTokenType.String:
                                    list.Add(val.Value<string>());
                                    break;
                                case JTokenType.Array:
                                    list = val.Values<string>().ToList();
                                    break;
                            }

                            errors[name] = list;
                        }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                errors[e.Message] = new List<string> { inputStr };
            }

            // KVPs are structs and can never be null. Instead, check if the first error equals the default kvp value.
            if (errors.FirstOrDefault().Equals(default(KeyValuePair<string, IEnumerable<string>>)))
                return null;

            return errors;
        }

        /// <summary>
        /// Executes a request and returns the raw result string. Throws an exception when the response is invalid.
        /// </summary>
        public static async Task<string> ExecuteRequestToStringAsync(BizwebRequestMessage requestMsg,
            IRequestExecutionPolicy execPolicy)
        {
            return await execPolicy.Run(requestMsg, async (reqMsg) =>
            {
                //Need to create a RequestInfo before send RequestMessage
                //because after that, HttpClient will dispose RequestMessage
                var requestInfo = await CreateRequestSimpleInfoAsync(reqMsg);

                //Make request
                var request = HttpUtils.CreateHttpClient().SendAsync(reqMsg);

                using (var response = await request)
                {
                    //Check for and throw exception when necessary.
                    await CheckResponseExceptionsAsync(response, requestInfo);

                    var rawResponse = await response.Content.ReadAsStringAsync();
                    return new RequestResult<string>(response, rawResponse);
                }
            });
        }

        /// <summary>
        /// Executes a request and returns the raw result string. Throws an exception when the response is invalid.
        /// </summary>
        public static async Task<string> ExecuteRequestToStringAsync(BizwebRequestMessage requestMsg)
        {
            return await ExecuteRequestToStringAsync(requestMsg, DefaultRequestExecutionPolicy.Default);
        }

        /// <summary>
        /// Executes a request and returns the JToken result. Throws an exception when the response is invalid.
        /// </summary>
        public static async Task<JToken> ExecuteRequestAsync(BizwebRequestMessage requestMsg,
            IRequestExecutionPolicy execPolicy)
        {
            var responseStr = await ExecuteRequestToStringAsync(requestMsg, execPolicy);

            // When using JToken make sure that dates are not stripped of any timezone information
            // if tokens are de-serialised into strings/DateTime/DateTimeZoneOffset
            using (var sr = new StringReader(string.IsNullOrEmpty(responseStr) ? "{}" : responseStr))
            using (var jr = new JsonTextReader(sr) { DateParseHandling = DateParseHandling.None })
            {
                return await JToken.ReadFromAsync(jr);
            }
        }

        /// <summary>
        /// Executes a request and returns the JToken result. Throws an exception when the response is invalid.
        /// </summary>
        public static async Task<JToken> ExecuteRequestAsync(BizwebRequestMessage requestMsg)
        {
            return await ExecuteRequestAsync(requestMsg, DefaultRequestExecutionPolicy.Default);
        }

        /// <summary>
        /// Executes a request and returns the given type. Throws an exception when the response is invalid.
        /// Use this method when the expected response is a single line or simple object that doesn't warrant its own class.
        /// </summary>
        public static async Task<T> ExecuteRequestAsync<T>(BizwebRequestMessage requestMsg,
            IRequestExecutionPolicy execPolicy)
            where T : new()
        {
            var rawResponse = await ExecuteRequestToStringAsync(requestMsg, execPolicy);
            return Deserialize<T>(rawResponse, requestMsg.RootElement);
        }

        /// <summary>
        /// Executes a request and returns the given type. Throws an exception when the response is invalid.
        /// Use this method when the expected response is a single line or simple object that doesn't warrant its own class.
        /// </summary>
        public static async Task<T> ExecuteRequestAsync<T>(BizwebRequestMessage requestMsg)
            where T : new()
        {
            return await ExecuteRequestAsync<T>(requestMsg, DefaultRequestExecutionPolicy.Default);
        }

        private static T Deserialize<T>(string rawResponse, string rootElement = null)
        {
            //Notice: deserialize can fails when response body null or empty
            //Create a default T or null ?
            var output = Activator.CreateInstance<T>();

            if (string.IsNullOrEmpty(rootElement))
            {
                output = JsonConvert.DeserializeObject<T>(rawResponse, Serializer.DeserializeSettings);
            }
            else
            {
                var data = JsonConvert.DeserializeObject(rawResponse, Serializer.DeserializeSettings) as JToken;

                if (data[rootElement] != null)
                    output = data[rootElement].ToObject<T>();
            }

            return output;
        }
    }
}