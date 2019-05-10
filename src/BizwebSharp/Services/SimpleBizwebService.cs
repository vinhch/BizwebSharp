using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    /// <summary>
    /// The service for simple call to Bizweb API.
    /// All results will be return as string.
    /// </summary>
    public class SimpleBizwebService : BaseService
    {
        /// <summary>
        /// Creates a new instance of <see cref="SimpleBizwebService" />.
        /// </summary>
        /// <param name="authState">The object contain Bizweb authorization data.</param>
        public SimpleBizwebService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Gets a results in API.
        /// </summary>
        /// <param name="apiPath">API query and path, not include domain.</param>
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

        /// <summary>
        /// Posts a payload data to API
        /// </summary>
        /// <param name="apiPath">API query and path, not include domain.</param>
        /// <param name="data">The payload data.</param>
        /// <param name="rootElement">The result root element.</param>
        /// <returns></returns>
        public async Task<string> PostAsync(string apiPath, object data, string rootElement = null)
        {
            return await PostOrPutAsync(HttpMethod.Post, apiPath, data, rootElement);
        }

        /// <summary>
        /// Puts a payload data to API
        /// </summary>
        /// <param name="apiPath">API query and path, not include domain.</param>
        /// <param name="data">The payload data.</param>
        /// <param name="rootElement">The result root element.</param>
        public async Task<string> PutAsync(string apiPath, object data, string rootElement = null)
        {
            return await PostOrPutAsync(HttpMethod.Put, apiPath, data, rootElement);
        }

        /// <summary>
        /// Deletes a data in API.
        /// </summary>
        /// <param name="apiPath">API query and path, not include domain.</param>
        public async Task DeleteAsync(string apiPath)
        {
            using (var req = RequestEngine.CreateRequest(_AuthState, apiPath, HttpMethod.Delete))
            {
                await RequestEngine.ExecuteRequestToStringAsync(req, ExecutionPolicy);
            }
        }
    }
}