using System;
using System.Threading.Tasks;

namespace BizwebSharp.Infrastructure
{
    public class LimitRetryExecutionPolicy : IRequestExecutionPolicy
    {
        private const byte MAX_RETRY = 10;
        private static readonly TimeSpan RETRY_DELAY = TimeSpan.FromMilliseconds(3000);

        public async Task<T> Run<T>(BizwebRequestMessage baseReqMsg,
            ExecuteRequestAsync<T> executeRequestAsync)
        {
            var tryCount = 0;
            Start:
            try
            {
                using (var reqMsg = baseReqMsg.Clone())
                {
                    tryCount++;
                    return (await executeRequestAsync(reqMsg)).Result;
                }
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