using System;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "RequestEngine")]
    public class RequestEngine_Tests : IDisposable
    {
        //private readonly IRestResponse _errorResponse;
        //private readonly IRestResponse _apiRateLimitResponse;
        private readonly HttpResponseMessage _errorResponse;
        private readonly HttpResponseMessage _apiRateLimitResponse;

        //Setup
        public RequestEngine_Tests()
        {
            //var req = new Moq.Mock<IRestResponse>();
            //req.Setup(r => r.StatusCode).Returns(System.Net.HttpStatusCode.InternalServerError);
            //req.Setup(r => r.RawBytes).Returns(null as byte[]);
            //req.Setup(r => r.StatusDescription).Returns("Internal Server Error");
            //_errorResponse = req.Object;

            //var req1 = new Moq.Mock<IRestResponse>();
            //req1.Setup(r => r.StatusCode).Returns((System.Net.HttpStatusCode)429);
            //req1.Setup(r => r.RawBytes).Returns(null as byte[]);
            //req1.Setup(r => r.StatusDescription).Returns("Too Many Requests");
            //_apiRateLimitResponse = req1.Object;

            _errorResponse = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                ReasonPhrase = "Internal Server Error"
            };

            _apiRateLimitResponse = new HttpResponseMessage
            {
                StatusCode = (System.Net.HttpStatusCode) 429,
                ReasonPhrase = "Too Many Requests"
            };
        }

        //Teardown
        public void Dispose()
        {
        }

        //Test
        [Fact(DisplayName = "When checking a null response for exceptions, it should return a message about the statuscode")]
        public async Task CheckNullResponse()
        {
            var exception = await Record.ExceptionAsync(() => RequestEngine.CheckResponseExceptionsAsync(_errorResponse));
            Assert.NotNull(exception);
            Assert.IsType<BizwebSharpException>(exception);

            var ex = (BizwebSharpException) exception;
            ex.HttpStatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
            ex.Message.Should().Contain("Response did not indicate success. Status: 500");
        }

        [Fact(DisplayName = "When having a api rate limit response, it should return an ApiRateLimitException")]
        public async Task CheckApiRateLimit()
        {
            var exception = await Record.ExceptionAsync(() => RequestEngine.CheckResponseExceptionsAsync(_apiRateLimitResponse));
            Assert.NotNull(exception);
            Assert.IsType<ApiRateLimitException>(exception);

            var ex = (ApiRateLimitException) exception;
            ex.HttpStatusCode.Should().Be((System.Net.HttpStatusCode)429);
            ex.Message.Should().Contain("Too Many Requests");
        }
    }
}
