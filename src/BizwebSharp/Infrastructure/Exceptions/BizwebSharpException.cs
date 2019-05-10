using System;
using System.Collections.Generic;
using System.Net;

namespace BizwebSharp.Infrastructure
{
    /// <summary>
    /// The class for all Bizweb API exceptional.
    /// </summary>
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

        /// <summary>
        /// The http error code returned by Bizweb.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        public Dictionary<string, IEnumerable<string>> Errors { get; set; } =
            new Dictionary<string, IEnumerable<string>>();

        /// <summary>
        /// The raw JSON string returned by Bizweb.
        /// </summary>
        public string RawBody { get; set; }

        /// <summary>
        /// An simple info of the request that was requested to Bizweb
        /// </summary>
        public RequestSimpleInfo RequestInfo { get; set; }

        /// <summary>
        /// The XRequestId header returned by Bizweb. Can be used when working with the Bizweb support team to identify the failed request.
        /// </summary>
        public string RequestId { get; set; }
    }
}