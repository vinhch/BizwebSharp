using System;
using System.Collections.Generic;
using System.Net;

namespace BizwebSharp.Infrastructure
{
    public class BizwebSharpException : Exception
    {
        public BizwebSharpException(string message) : base(message)
        {
        }

        public BizwebSharpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BizwebSharpException(HttpStatusCode httpStatusCode, Dictionary<string, IEnumerable<string>> errors,
            string message, string rawBody, RequestSimpleInfo requestInfo = null) : base(message)
        {
            HttpStatusCode = httpStatusCode;
            Errors = errors;
            RawBody = rawBody;
            RequestInfo = requestInfo;
        }

        public HttpStatusCode HttpStatusCode { get; set; }

        public Dictionary<string, IEnumerable<string>> Errors { get; set; } =
            new Dictionary<string, IEnumerable<string>>();

        public string RawBody { get; set; }

        public RequestSimpleInfo RequestInfo { get; set; }
    }
}