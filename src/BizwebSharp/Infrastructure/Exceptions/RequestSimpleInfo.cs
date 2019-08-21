namespace BizwebSharp.Infrastructure
{
    /// <summary>
    /// The class contain simple request info that was requested to Bizweb
    /// </summary>
    public class RequestSimpleInfo
    {
        /// <summary>
        /// The URL of request
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The Http method of request
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The Http header of request
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// The Http body of request
        /// </summary>
        public string Body { get; set; }
    }
}