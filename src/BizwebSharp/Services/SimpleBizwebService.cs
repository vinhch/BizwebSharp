using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    public class SimpleBizwebService : BaseService
    {
        public SimpleBizwebService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public async Task<string> GetAsync(string apiPath)
        {
            using (var req = RequestEngine.CreateRequest(_AuthState, apiPath, HttpMethod.Get))
            {
                return await RequestEngine.ExecuteRequestToStringAsync(req, ExecutionPolicy);
            }
        }

        private async Task<string> PostOrPutAsync(HttpMethod method, string apiPath, object data, string rootElement = null)
        {
            JsonContent content = null;
            if (string.IsNullOrEmpty(rootElement))
            {
                content = new JsonContent(data);
            }
            else
            {
                var body = new Dictionary<string, object>
                {
                    {rootElement, data}
                };
                content = new JsonContent(body);
            }

            using (var req = RequestEngine.CreateRequest(_AuthState, apiPath, method, content, rootElement))
            {
                return await RequestEngine.ExecuteRequestToStringAsync(req, ExecutionPolicy);
            }
        }

        public async Task<string> PostAsync(string apiPath, object data, string rootElement = null)
        {
            return await PostOrPutAsync(HttpMethod.Post, apiPath, data, rootElement);
        }

        public async Task<string> PutAsync(string apiPath, object data, string rootElement = null)
        {
            return await PostOrPutAsync(HttpMethod.Put, apiPath, data, rootElement);
        }

        public async Task DeleteAsync(string apiPath)
        {
            using (var req = RequestEngine.CreateRequest(_AuthState, apiPath, HttpMethod.Delete))
            {
                await RequestEngine.ExecuteRequestToStringAsync(req, ExecutionPolicy);
            }
        }
    }
}