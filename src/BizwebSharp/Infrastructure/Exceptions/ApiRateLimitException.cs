using System;
using System.Collections.Generic;
using System.Net;

namespace BizwebSharp.Infrastructure
{
    /// <summary>
    /// An exception thrown when an API call has reached Bizweb's rate limit.
    /// </summary>
    public class ApiRateLimitException : BizwebSharpException
    {
        public ApiRateLimitException(string message) : base(message)
        {
        }

        public ApiRateLimitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ApiRateLimitException(HttpStatusCode httpStatusCode, Dictionary<string, IEnumerable<string>> errors,
            string message, string rawBody, RequestSimpleInfo requestInfo = null)
            : base(httpStatusCode, errors, message, rawBody, requestInfo)
        {
        }
    }
}