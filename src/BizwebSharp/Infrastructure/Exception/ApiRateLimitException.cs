using System;
using System.Collections.Generic;
using System.Net;

namespace BizwebSharp.Infrastructure
{
    public class ApiRateLimitException : CustomApiException
    {
        public ApiRateLimitException(string message) : base(message)
        {
        }

        public ApiRateLimitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ApiRateLimitException(HttpStatusCode httpStatusCode, Dictionary<string, IEnumerable<string>> errors,
            string message, string jsonError, RequestSimpleInfo requestInfo = null)
            : base(httpStatusCode, errors, message, jsonError, requestInfo)
        {
        }
    }
}