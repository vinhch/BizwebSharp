using System;
using System.Linq;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "BizwebSharpException")]
    public class BizwebSharpException_Tests
    {
        [Fact(DisplayName = "When reaching the rate limit, It should throw a rate limit exception")]
        public async Task Catches_Rate_Limit()
        {
            const int requestCount = 60;
            var service = new StoreService(Utils.AuthState)
            {
                ExecutionPolicy = new DefaultRequestExecutionPolicy()
            };
            ApiRateLimitException ex = null;
            var i = 0;

            try
            {
                /*
                 * with Task.WhenAll and requestCount >= 61 will catch timeout?
                 * but why if requestCount <= 60 will not throw any exception, include ApiRateLimitException ??
                 * windows or .net framework limit ?
                 */

                //var tasks = Enumerable.Range(0, requestCount).Select(_ => service.GetAsync("id"));
                //await Task.WhenAll(tasks);

                while (i < requestCount)
                {
                    i++;
                    await service.GetAsync("id");
                }
            }
            catch (ApiRateLimitException e)
            {
                ex = e;
            }

            Assert.NotNull(ex);
            Assert.Equal(429, (int)ex.HttpStatusCode);
            Assert.NotNull(ex.RawBody);
            Assert.True(ex.Errors.Count > 0);
            //Assert.Equal("Error", ex.Errors.First().Key);
            //Assert.Equal(
            //    "Exceeded 2 calls per second for api client. Reduce request rates to resume uninterrupted service.",
            //    ex.Errors.First().Value.First());
        }
    }
}
