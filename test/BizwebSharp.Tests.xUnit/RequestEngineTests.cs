using System;
using BizwebSharp.Infrastructure;
using RestSharp.Portable;
using Xunit;
using FluentAssertions;

namespace BizwebSharp.Tests
{
    public class RequestEngineTests : IDisposable
    {
        private readonly IRestResponse ErrorResponse;
        private readonly IRestResponse ApiRateLimitResponse;

        //Setup
        public RequestEngineTests()
        {
            var req = new Moq.Mock<IRestResponse>();
            req.Setup(r => r.StatusCode).Returns(System.Net.HttpStatusCode.InternalServerError);
            req.Setup(r => r.RawBytes).Returns(null as byte[]);
            req.Setup(r => r.StatusDescription).Returns("Internal Server Error");
            ErrorResponse = req.Object;

            var req1 = new Moq.Mock<IRestResponse>();
            req1.Setup(r => r.StatusCode).Returns((System.Net.HttpStatusCode)429);
            req1.Setup(r => r.RawBytes).Returns(null as byte[]);
            req1.Setup(r => r.StatusDescription).Returns("Too Many Requests");
            ApiRateLimitResponse = req1.Object;
        }

        //Teardown
        public void Dispose()
        {
        }

        //Test
        [Fact(DisplayName = "When checking a null response for exceptions, it should return a message about the statuscode")]
        public void CheckNullResponse()
        {
            var exception = Record.Exception(() => RequestEngine.CheckResponseExceptions(ErrorResponse));
            Assert.NotNull(exception);
            Assert.IsType<CustomApiException>(exception);

            var ex = (CustomApiException) exception;
            ex.HttpStatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
            ex.Message.Should().Contain("Response did not indicate success. Status: 500");
        }

        [Fact(DisplayName = "When having a api rate limit response, it should return an ApiRateLimitException")]
        public void CheckApiRateLimit()
        {
            var exception = Record.Exception(() => RequestEngine.CheckResponseExceptions(ApiRateLimitResponse));
            Assert.NotNull(exception);
            Assert.IsType<ApiRateLimitException>(exception);

            var ex = (ApiRateLimitException) exception;
            ex.HttpStatusCode.Should().Be((System.Net.HttpStatusCode)429);
            ex.Message.Should().Contain("Too Many Requests");
        }
    }
}
