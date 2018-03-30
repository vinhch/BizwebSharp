using System;
using System.Net.Http;

namespace BizwebSharp.Infrastructure
{
    public class BizwebRequestMessage : HttpRequestMessage
    {
        public string RootElement { get; set; }

        public BizwebRequestMessage(Uri url, HttpMethod method,
            HttpContent content = null, string rootElement = null) : base(method, url)
        {
            if (content != null)
            {
                this.Content = content;
            }

            RootElement = rootElement;
        }

        public BizwebRequestMessage Clone()
        {
            var newContent = Content;

            if (newContent != null && newContent is JsonContent c)
            {
                newContent = c.Clone();
            }

            var cloned = new BizwebRequestMessage(RequestUri, Method, newContent, RootElement);

            // Copy over the request's headers which includes the access token if set
            foreach (var header in Headers)
            {
                cloned.Headers.Add(header.Key, header.Value);
            }

            return cloned;
        }
    }
}
