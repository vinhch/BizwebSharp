using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using RestSharp.Portable;

namespace BizwebSharp.Services
{
    public class SimpleBizwebService : BaseService
    {
        public SimpleBizwebService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public async Task<string> GetAsync(string apiPath)
        {
            var req = RequestEngine.CreateRequest(apiPath, Method.GET);
            return await RequestEngine.ExecuteRequestToStringAsync(_RestClient, req);
        }

        private async Task<object> PostOrPutAsync(Method method, string apiPath, object data, string rootElement = null)
        {
            var req = RequestEngine.CreateRequest(apiPath, method, rootElement);
            if (string.IsNullOrEmpty(rootElement))
            {
                req.AddJsonBody(data);
            }
            else
            {
                var body = new Dictionary<string, object>
                {
                    {rootElement, data}
                };
                req.AddJsonBody(body);
            }
            return await RequestEngine.ExecuteRequestToStringAsync(_RestClient, req);
        }

        public async Task<object> PostAsync(string apiPath, object data, string rootElement = null)
        {
            return await PostOrPutAsync(Method.POST, apiPath, data, rootElement);
        }

        public async Task<object> PutAsync(string apiPath, object data, string rootElement = null)
        {
            return await PostOrPutAsync(Method.PUT, apiPath, data, rootElement);
        }

        public async Task DeleteAsync(string apiPath)
        {
            var req = RequestEngine.CreateRequest(apiPath, Method.DELETE);
            await RequestEngine.ExecuteRequestToStringAsync(_RestClient, req);
        }
    }
}