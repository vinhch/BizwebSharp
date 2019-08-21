using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using Newtonsoft.Json.Linq;

namespace BizwebSharp
{
    /// <summary>
    /// Base class for Bizweb API services.
    /// All Bizweb API services should inherit from this class so that we can
    /// apply share base request method, authorization state and execution policy ...
    /// </summary>
    public abstract class BaseService
    {
        /// <summary>
        /// Creates a new instance of BizwebService.
        /// </summary>
        /// <param name="authState">The object contain Bizweb authorization data.</param>
        protected BaseService(BizwebAuthorizationState authState)
        {
            _AuthState = authState;
        }

        protected BizwebAuthorizationState _AuthState { get; }

        /// <summary>
        /// The execution policy for service
        /// </summary>
        public IRequestExecutionPolicy ExecutionPolicy { get; set; } = DefaultRequestExecutionPolicy.Default;

        private static BizwebRequestMessage CreateRequestMessage(BizwebAuthorizationState authState, string path,
            HttpMethod method, string rootElement = null, object payload = null)
        {
            JsonContent content = null;

            if (payload == null)
            {
                return RequestEngine.CreateRequest(authState, path, method, rootElement: rootElement);
            }

            if (method != HttpMethod.Get && method != HttpMethod.Delete)
            {
                content = new JsonContent(payload);
            }
            else if (payload is Parameterizable)
            {
                path = RequestEngine.CreateUriPathAndQuery(path, ((Parameterizable) payload).ToParameters());
            }
            else
            {
                //foreach (var propertyInfo in payload.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                //{
                //    req.AddParameter(propertyInfo.Name, propertyInfo.GetValue(payload));
                //}

                var token = JToken.FromObject(payload);
                var queryParams = token.Select(s =>
                {
                    var i = (JProperty) s;
                    return new KeyValuePair<string, object>(i.Name, i.Value);
                });

                path = RequestEngine.CreateUriPathAndQuery(path, queryParams);
            }

            return RequestEngine.CreateRequest(authState, path, method, content, rootElement);
        }

        protected async Task<T> MakeRequestAsync<T>(string path, HttpMethod httpMethod, string rootElement = null,
            object payload = null) where T : new()
        {
            using (var reqMsg = CreateRequestMessage(_AuthState, path, httpMethod, rootElement, payload))
            {
                return await RequestEngine.ExecuteRequestAsync<T>(reqMsg, ExecutionPolicy);
            }
        }

        protected async Task MakeRequestAsync(string path, HttpMethod httpMethod, string rootElement = null,
            object payload = null)
        {
            using (var reqMsg = CreateRequestMessage(_AuthState, path, httpMethod, rootElement, payload))
            {
                await RequestEngine.ExecuteRequestAsync(reqMsg, ExecutionPolicy);
            }
        }
    }
}