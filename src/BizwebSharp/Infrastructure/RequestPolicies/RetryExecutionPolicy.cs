using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BizwebSharp.Infrastructure
{
    /// <summary>
    ///     See https://help.shopify.com/api/guides/api-call-limit
    /// </summary>
    public class RetryExecutionPolicy : IRequestExecutionPolicy
    {
        private static readonly TimeSpan RETRY_DELAY = TimeSpan.FromMilliseconds(3000);

        public async Task<T> Run<T>(BizwebRequestMessage baseReqMsg,
            ExecuteRequestAsync<T> executeRequestAsync)
        {
            while (true)
            {
                try
                {
                    using (var reqMsg = baseReqMsg.Clone())
                    {
                        var fullResult = await executeRequestAsync(reqMsg);
                        return fullResult.Result;
                    }
                }
                catch (ApiRateLimitException)
                {
                    await Task.Delay(RETRY_DELAY);
                }
            }
        }
    }
}