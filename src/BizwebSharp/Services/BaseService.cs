using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using Newtonsoft.Json.Linq;

namespace BizwebSharp
{
    public abstract class BaseService
    {
        protected BaseService(BizwebAuthorizationState authState)
        {
            _AuthState = authState;
        }

        protected BizwebAuthorizationState _AuthState { get; }

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