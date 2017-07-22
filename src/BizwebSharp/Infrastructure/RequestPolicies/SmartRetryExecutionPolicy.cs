﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using RestSharp.Portable;

namespace BizwebSharp.Infrastructure
{
    /// <summary>
    /// A retry policy that attemps to pro-actively limit the number of requests that will result in a ShopifyRateLimitException
    /// by implementing the leaky bucket algorithm.
    /// For example: if 100 requests are created in parallel, only 40 should be immediately sent, and the subsequent 60 requests
    /// should be throttled at 1 per 500ms.
    /// </summary>
    /// <remarks>
    /// In comparison, the naive retry policy will issue the 100 requests immediately:
    /// 60 requests will fail and be retried after 500ms,
    /// 59 requests will fail and be retried after 500ms,
    /// 58 requests will fail and be retried after 500ms.
    /// See https://help.shopify.com/api/guides/api-call-limit
    /// https://en.wikipedia.org/wiki/Leaky_bucket
    /// </remarks>
    public partial class SmartRetryExecutionPolicy : IRequestExecutionPolicy
    {
        private static readonly TimeSpan THROTTLE_DELAY = TimeSpan.FromMilliseconds(500);

        private static readonly ConcurrentDictionary<string, LeakyBucket> _shopAccessTokenToLeakyBucket = new ConcurrentDictionary<string, LeakyBucket>();

        public async Task<T> Run<T>(IRestClient baseClient, ICustomRestRequest request, ExecuteRequestAsync<T> executeRequestAsync)
        {
            var accessToken = GetAccessToken(baseClient);
            LeakyBucket bucket = null;

            if (accessToken != null)
            {
                bucket = _shopAccessTokenToLeakyBucket.GetOrAdd(accessToken, _ => new LeakyBucket());
            }

            while (true)
            {
                if (accessToken != null)
                {
                    await bucket.GrantAsync();
                }

                try
                {
                    var fullResult = await executeRequestAsync(baseClient);
                    var bucketContentSize = GetBucketContentSize(fullResult.Response);

                    if (bucketContentSize != null)
                    {
                        bucket?.SetContentSize(bucketContentSize.Value);
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
                    //-Multiple instance of the same program may use the same access token
                    await Task.Delay(THROTTLE_DELAY);
                }
            }
        }

        private static string GetAccessToken(IRestClient client)
        {
            return client.DefaultParameters
                .SingleOrDefault(p => p.Type == ParameterType.HttpHeader &&
                                      string.Equals(p.Name, ApiConst.HEADER_KEY_ACCESS_TOKEN,
                                          StringComparison.CurrentCultureIgnoreCase))
                ?.Value
                ?.ToString();
        }

        private static int? GetBucketContentSize(IRestResponse response)
        {
            var headers = response.Headers.FirstOrDefault(kvp => string.Equals(kvp.Key, ApiConst.HEADER_API_CALL_LIMIT,
                StringComparison.CurrentCultureIgnoreCase));

            var apiCallLimitHeaderValue = headers.Value?.FirstOrDefault();

            if (apiCallLimitHeaderValue != null)
            {
                if (int.TryParse(apiCallLimitHeaderValue.Split('/').First(), out int bucketContentSize))
                {
                    return bucketContentSize;
                }
            }

            return null;
        }
    }
}