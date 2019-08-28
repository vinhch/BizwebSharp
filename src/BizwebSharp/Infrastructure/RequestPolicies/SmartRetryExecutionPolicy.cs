using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BizwebSharp.Infrastructure
{
    /// <summary>
    /// A retry policy that attemps to pro-actively limit the number of requests that will result in a ApiRateLimitException
    /// by implementing the leaky bucket algorithm.
    /// For example: if 100 requests are created in parallel, only 40 should be immediately sent, and the subsequent 60 requests
    /// should be throttled at 2 per 1000ms.
    /// </summary>
    /// <remarks>
    /// In comparison, the naive retry policy will issue the 100 requests immediately:
    /// 60 requests will fail and be retried after 1000ms,
    /// 58 requests will fail and be retried after 1000ms,
    /// 56 requests will fail and be retried after 1000ms.
    /// See https://help.shopify.com/api/guides/api-call-limit
    /// https://en.wikipedia.org/wiki/Leaky_bucket
    /// </remarks>
    public partial class SmartRetryExecutionPolicy : IRequestExecutionPolicy
    {
        private static readonly TimeSpan THROTTLE_DELAY = TimeSpan.FromMilliseconds(1000);

        public async Task<T> Run<T>(BizwebRequestMessage baseReqMsg,
            ExecuteRequestAsync<T> executeRequestAsync)
        {
            var accessToken = GetAccessToken(baseReqMsg);
            LeakyBucket bucket = null;

            if (accessToken != null)
            {
                bucket = LeakyBucket.GetBucketByToken(accessToken);
            }

            while (true)
            {
                using (var reqMsg = baseReqMsg.Clone())
                {
                    if (accessToken != null)
                    {
                        await bucket.GrantAsync();
                    }

                    try
                    {
                        var fullResult = await executeRequestAsync(reqMsg);
                        var bucketState = GetBucketState(fullResult.Response);
                        var reportedFillLevel = bucketState.Item1;
                        var reportedCapacity = bucketState.Item2;

                        if (reportedFillLevel != null && reportedCapacity != null)
                        {
                            bucket?.SetBucketState(reportedFillLevel.Value, reportedCapacity.Value);
                        }

                        return fullResult.Result;
                    }
                    catch (BizwebSharpException)
                    {
                        //An exception may still occur:
                        //-Shopify may have a slightly different algorithm
                        //-Shopify may change to a different algorithm in the future
                        //-There may be timing and latency delays
                        //-Multiple programs may use the same access token
                        //-Multiple instances of the same program may use the same access token
                        await Task.Delay(THROTTLE_DELAY);
                    }
                }
            }
        }

        private static string GetAccessToken(HttpRequestMessage requestMsg)
        {
            return requestMsg.Headers
                .Where(p => string.Equals(p.Key, ApiConst.HEADER_KEY_ACCESS_TOKEN,
                    StringComparison.CurrentCultureIgnoreCase))
                .Select(p => new {p.Key, p.Value})
                .SingleOrDefault()
                ?.Value
                ?.FirstOrDefault();
        }

        private static Tuple<int?, int?> GetBucketState(HttpResponseMessage responseMsg)
        {
            var headers = responseMsg.Headers.FirstOrDefault(kvp =>
                string.Equals(kvp.Key, ApiConst.HEADER_API_CALL_LIMIT, StringComparison.CurrentCultureIgnoreCase));
            var apiCallLimitHeaderValue = headers.Value?.FirstOrDefault();
            if (apiCallLimitHeaderValue == null)
            {
                return Tuple.Create<int?, int?>(null, null);
            }

            var split = apiCallLimitHeaderValue.Split('/');
            if (split.Length == 2 &&
                int.TryParse(split[0], out var reportedFillLevel) &&
                int.TryParse(split[1], out var reportedCapacity))
            {
                return Tuple.Create<int?, int?>(reportedFillLevel, reportedCapacity);
            }

            return Tuple.Create<int?, int?>(null, null);
        }
    }
}
