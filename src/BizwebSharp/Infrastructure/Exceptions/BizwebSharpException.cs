﻿using System;
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
            string message, string jsonError, RequestSimpleInfo requestInfo = null) : base(message)
        {
            HttpStatusCode = httpStatusCode;
            Errors = errors;
            JsonError = jsonError;
            RequestInfo = requestInfo;
        }

        public HttpStatusCode HttpStatusCode { get; set; }

        public Dictionary<string, IEnumerable<string>> Errors { get; set; } =
            new Dictionary<string, IEnumerable<string>>();

        public string JsonError { get; set; }

        public RequestSimpleInfo RequestInfo { get; set; }
    }
}