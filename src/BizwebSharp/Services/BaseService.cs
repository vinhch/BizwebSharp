using System;
using System.Reflection;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using RestSharp.Portable;

namespace BizwebSharp.Services
{
    public abstract class BaseService
    {
        public BaseService(BizwebAuthorizationState authState)
        {
            _AuthState = authState;
        }

        protected BizwebAuthorizationState _AuthState { get; }

        private static ICustomRestRequest CreateRestRequest(string path, HttpMethod httpMethod, string rootElement = null,
            object payload = null)
        {
            var method = HttpMethodToRestSharpMethod(httpMethod);
            var req = RequestEngine.CreateRequest(path, method, rootElement);

            if (payload == null) return req;

            if (method != Method.GET && method != Method.DELETE)
            {
                req.AddJsonBody(payload);
            }
            else if (payload is Parameterizable)
            {
                foreach (var parameter in ((Parameterizable)payload).ToParameters(ParameterType.GetOrPost))
                {
                    req.Parameters.Add(parameter);
                }
            }
            else
            {
                foreach (var propertyInfo in payload.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    req.AddParameter(propertyInfo.Name, propertyInfo.GetValue(payload));
                }
            }

            return req;
        }

        protected async Task<T> MakeRequest<T>(string path, HttpMethod httpMethod, string rootElement = null,
            object payload = null) where T : new()
        {
            var req = CreateRestRequest(path, httpMethod, rootElement, payload);
            using (var client = RequestEngine.CreateClient(_AuthState))
            {
                return await RequestEngine.ExecuteRequestAsync<T>(client, req);
            }
        }

        protected async Task MakeRequest(string path, HttpMethod httpMethod, string rootElement = null,
            object payload = null)
        {
            var req = CreateRestRequest(path, httpMethod, rootElement, payload);
            using (var client = RequestEngine.CreateClient(_AuthState))
            {
                await RequestEngine.ExecuteRequestAsync(client, req);
            }
        }

        private static Method HttpMethodToRestSharpMethod(HttpMethod method)
        {
            var methodName = Enum.GetName(typeof(HttpMethod), method);

            Method result;
            if (!Enum.TryParse(methodName, out result))
            {
                throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }

            return result;
        }
    }
}