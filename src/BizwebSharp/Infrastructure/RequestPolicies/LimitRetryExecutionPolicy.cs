using System;
using System.Threading.Tasks;
using RestSharp.Portable;

namespace BizwebSharp.Infrastructure.RequestPolicies
{
    public class LimitRetryExecutionPolicy : IRequestExecutionPolicy
    {
        private const byte MAX_RETRY = 10;
        private static readonly TimeSpan RETRY_DELAY = TimeSpan.FromMilliseconds(3000);

        public async Task<T> Run<T>(IRestClient client, ICustomRestRequest request,
            ExecuteRequestAsync<T> executeRequestAsync)
        {
            var tryCount = 0;
            Start:
            try
            {
                tryCount++;
                return (await executeRequestAsync(client)).Result;
            }
            catch (ApiRateLimitException)
            {
                if (tryCount >= MAX_RETRY) throw;
                await Task.Delay(RETRY_DELAY);
                goto Start;
            }
        }
    }
}