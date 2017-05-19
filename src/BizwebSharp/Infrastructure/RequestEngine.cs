using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Const;
using BizwebSharp.Infrastructure.RequestPolicies;
using BizwebSharp.Serializers;
using Newtonsoft.Json.Linq;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;

namespace BizwebSharp.Infrastructure
{
    public static class RequestEngine
    {
        private static IRequestExecutionPolicy _executionPolicy = new LimitRetryExecutionPolicy();

        public static void SetExecutionPolicy(IRequestExecutionPolicy executionPolicy)
        {
            _executionPolicy = executionPolicy;
        }

        public static Uri BuildUri(string myApiUrl, bool usingHttps = true)
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
                Path = "admin/",
                Scheme = protocolScheme,
                Port = usingHttps ? 443 : 80 //default SSL port
            };

            return builder.Uri;
        }

        public static IRestClient CreateClient(BizwebAuthorizationState authState)
        {
            var uri = BuildUri(authState.ApiUrl);
            var client = new RestClient(uri);

            //Set up the JSON.NET deserializer for the RestSharp client
            //var deserializer = new JsonNetSerializer();
            //client.ContentHandlers.Add("application/json", deserializer);
            //client.ContentHandlers.Add("text/json", deserializer);

            if (!string.IsNullOrEmpty(authState.AccessToken))
            {
                client.AddDefaultParameter(ApiConst.HeaderKeyAccessToken, authState.AccessToken,
                    ParameterType.HttpHeader);
            }

            return client;
        }

        public static ICustomRestRequest CreateRequest(string path, Method method, string rootElement = null)
        {
            return new CustomRestRequest(path, method)
            {
                Serializer = new JsonNetSerializer(),
                RootElement = rootElement
            };
        }

        public static void CheckResponseExceptions(IRestResponse response)
        {
            if ((response.StatusCode != HttpStatusCode.OK) && (response.StatusCode != HttpStatusCode.Created))
            {
                var json = response.Content;
                var errors = ParseErrorJson(json);
                var requestInfo = new RequestSimpleInfo(response);

                var code = response.StatusCode;
                var message = $"Response did not indicate success. Status: {(int) code} {response.StatusDescription}.";

                if (errors == null)
                {
                    errors = new Dictionary<string, IEnumerable<string>>
                    {
                        {
                            $"{(int) code} {response.StatusDescription}",
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
                if ((int) code == 429 /* Too many requests */)
                    throw new ApiRateLimitException(code, errors, message, json, requestInfo);

                throw new BizwebSharpException(code, errors, message, json, requestInfo);
            }

            //if (response.ErrorException != null)
            //{
            //    //Checking this second, because Shopify errors sometimes return incomplete objects along with errors,
            //    //which cause Json deserialization to throw an exception. Parsing the Shopify error is more important
            //    //than throwing this deserialization exception.
            //    throw response.ErrorException;
            //}
        }

        private static Dictionary<string, IEnumerable<string>> ParseErrorJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var errors = new Dictionary<string, IEnumerable<string>>();

            try
            {
                var parsed = JToken.Parse(string.IsNullOrEmpty(json) ? "{}" : json);

                // Errors can be any of the following:
                // 1. { errors: "some error message"}
                // 2. { errors: { "order" : "some error message" } }
                // 3. { errors: { "order" : [ "some error message" ] } }
                // 4. { error: "invalid_request", error_description:"The authorization code was not found or was already used" }

                if (parsed.Any(p => p.Path == "error") && parsed.Any(p => p.Path == "error_description"))
                {
                    // Error is type #4
                    var description = parsed["error_description"];

                    errors.Add("invalid_request", new List<string> {description.Value<string>()});
                }
                else if (parsed.Any(x => x.Path == "errors"))
                {
                    var parsedErrors = parsed["errors"];

                    //errors can be either a single string, or an array of other errors
                    if (parsedErrors.Type == JTokenType.String)
                        errors.Add("Error", new List<string> {parsedErrors.Value<string>()});
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

                            errors.Add(name, list);
                        }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                errors.Add(e.Message, new List<string> {json});
            }

            // KVPs are structs and can never be null. Instead, check if the first error equals the default kvp value.
            if (errors.FirstOrDefault().Equals(default(KeyValuePair<string, IEnumerable<string>>)))
                return null;

            return errors;
        }

        public static async Task<string> ExecuteRequestToStringAsync(IRestClient baseClient, ICustomRestRequest request)
        {
            return await _executionPolicy.Run(baseClient, request, async (client) =>
            {
                //Make request
                var response = await client.Execute(request);

                //Check for and throw exception when necessary.
                CheckResponseExceptions(response);

                return new RequestResult<string>(response, response.Content);
            });
        }

        public static async Task<JToken> ExecuteRequestAsync(IRestClient baseClient, ICustomRestRequest request)
        {
            var responseStr = await ExecuteRequestToStringAsync(baseClient, request);
            return JToken.Parse(string.IsNullOrEmpty(responseStr) ? "{}" : responseStr);
        }

        public static async Task<T> ExecuteRequestAsync<T>(IRestClient baseClient, ICustomRestRequest request)
            where T : new()
        {
            return await _executionPolicy.Run(baseClient, request, async (client) =>
            {
                //Make request
                var response = await client.Execute(request);

                //Check for and throw exception when necessary.
                CheckResponseExceptions(response);

                var deserializer = new JsonNetSerializer
                {
                    RootElement = request.RootElement
                };
                var result = deserializer.Deserialize<T>(response);

                return new RequestResult<T>(response, result);
            });
        }
    }
}