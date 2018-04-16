using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BizwebSharp.Infrastructure
{
    public static class RequestEngine
    {
        //HttpClient instance need to be singleton because of this https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        private static HttpClient _httpClient = new HttpClient();
        public static HttpClient CurrentHttpClient => _httpClient ?? (_httpClient = new HttpClient());

        public static string CreateUriPathAndQuery(string path, IEnumerable<KeyValuePair<string, object>> queryParams)
        {
            var query = queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value.ToString())}");
            return $"{path}?{string.Join("&", query)}";
        }

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

        public static BizwebRequestMessage CreateRequest(BizwebAuthorizationState authState, string pathAndQuery,
            HttpMethod method, HttpContent content = null, string rootElement = null)
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

        public static async Task CheckResponseExceptionsAsync(HttpResponseMessage resMsg, RequestSimpleInfo requestInfo = null)
        {
            var statusCode = (int)resMsg.StatusCode;
            if (statusCode >= 200 && statusCode < 300)
            {
                return;
            }

            string rawResponse = null;
            if (resMsg.Content != null)
            {
                rawResponse = await resMsg.Content.ReadAsStringAsync();
            }

            var errors = ParseErrorJson(rawResponse);

            var message = $"Response did not indicate success. Status: {statusCode} {resMsg.ReasonPhrase}.";

            if (errors == null)
            {
                errors = new Dictionary<string, IEnumerable<string>>
                {
                    {
                        $"{statusCode} {resMsg.ReasonPhrase}",
                        new[] {message}
                    }
                };
            }
            else
            {
                var firstError = errors.First();

                message = $"{firstError.Key}: {string.Join(", ", firstError.Value)}";
            }

            // If the error was caused by reaching the API rate limit, throw a rate limit exception.
            if (statusCode == 429 /* Too many requests */)
            {
                throw new ApiRateLimitException(resMsg.StatusCode, errors, message, rawResponse, requestInfo);
            }

            throw new BizwebSharpException(resMsg.StatusCode, errors, message, rawResponse, requestInfo);

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

        public static async Task<string> ExecuteRequestToStringAsync(BizwebRequestMessage requestMsg,
            IRequestExecutionPolicy execPolicy)
        {
            return await execPolicy.Run(CurrentHttpClient, requestMsg, async (client, reqMsg) =>
            {
                //Need to create a RequestInfo before send RequestMessage
                //because after that, HttpClient will dispose RequestMessage
                var requestInfo = await CreateRequestSimpleInfoAsync(reqMsg);

                //Make request
                var request = client.SendAsync(reqMsg);

                using (var response = await request)
                {
                    //Check for and throw exception when necessary.
                    await CheckResponseExceptionsAsync(response, requestInfo);

                    var rawResponse = await response.Content.ReadAsStringAsync();
                    return new RequestResult<string>(response, rawResponse);
                }
            });
        }

        public static async Task<string> ExecuteRequestToStringAsync(BizwebRequestMessage requestMsg)
        {
            return await ExecuteRequestToStringAsync(requestMsg, DefaultRequestExecutionPolicy.Default);
        }

        public static async Task<JToken> ExecuteRequestAsync(BizwebRequestMessage requestMsg,
            IRequestExecutionPolicy execPolicy)
        {
            var responseStr = await ExecuteRequestToStringAsync(requestMsg, execPolicy);
            return JToken.Parse(string.IsNullOrEmpty(responseStr) ? "{}" : responseStr);
        }

        public static async Task<JToken> ExecuteRequestAsync(BizwebRequestMessage requestMsg)
        {
            return await ExecuteRequestAsync(requestMsg, DefaultRequestExecutionPolicy.Default);
        }

        public static async Task<T> ExecuteRequestAsync<T>(BizwebRequestMessage requestMsg,
            IRequestExecutionPolicy execPolicy)
            where T : new()
        {
            return await execPolicy.Run(CurrentHttpClient, requestMsg, async (client, reqMsg) =>
            {
                //Need to create a RequestInfo before send RequestMessage
                //because after that, HttpClient will dispose RequestMessage
                var requestInfo = await CreateRequestSimpleInfoAsync(reqMsg);

                //Make request
                var request = client.SendAsync(reqMsg);

                using (var response = await request)
                {
                    //Check for and throw exception when necessary.
                    await CheckResponseExceptionsAsync(response, requestInfo);

                    //Notice: deserialize can fails when response body null or empty
                    var rawResponse = await response.Content.ReadAsStringAsync();
                    var result = Deserialize<T>(rawResponse, reqMsg.RootElement);

                    return new RequestResult<T>(response, result);
                }
            });
        }

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

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            //if (!string.IsNullOrEmpty(DateFormat))
            //{
            //    settings.DateFormatString = DateFormat;
            //}

            if (string.IsNullOrEmpty(rootElement))
            {
                output = JsonConvert.DeserializeObject<T>(rawResponse, settings);
            }
            else
            {
                var data = JsonConvert.DeserializeObject(rawResponse, settings) as JToken;

                if (data[rootElement] != null)
                    output = data[rootElement].ToObject<T>();
            }

            return output;
        }
    }
}