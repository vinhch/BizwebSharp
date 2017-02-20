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
            _RestClient = RequestEngine.CreateClient(authState);
        }

        protected IRestClient _RestClient { get; }

        private static ICustomRestRequest CreateRestRequest(string path, HttpMethod httpMethod, string rootElement = null,
            object payload = null)
        {
            var method = GetMethod(httpMethod);
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
            return await RequestEngine.ExecuteRequestAsync<T>(_RestClient, req);
        }

        protected async Task MakeRequest(string path, HttpMethod httpMethod, string rootElement = null,
            object payload = null)
        {
            var req = CreateRestRequest(path, httpMethod, rootElement, payload);
            await RequestEngine.ExecuteRequestAsync(_RestClient, req);
        }

        private static Method GetMethod(HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.GET:
                    return Method.GET;
                case HttpMethod.POST:
                    return Method.POST;
                case HttpMethod.PUT:
                    return Method.PUT;
                case HttpMethod.DELETE:
                    return Method.DELETE;
                case HttpMethod.HEAD:
                    return Method.HEAD;
                case HttpMethod.OPTIONS:
                    return Method.OPTIONS;
                case HttpMethod.PATCH:
                    return Method.PATCH;
                case HttpMethod.MERGE:
                    return Method.MERGE;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }
    }
}