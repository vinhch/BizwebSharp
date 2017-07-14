using System.Linq;
using RestSharp.Portable;

namespace BizwebSharp.Infrastructure
{
    public class RequestSimpleInfo
    {
        public RequestSimpleInfo()
        {
        }

        public RequestSimpleInfo(IRestResponse response)
        {
            if (response.Request == null) return;

            Url = response.ResponseUri.AbsoluteUri;
            Method = response.Request.Method.ToString();

            if (response.Request.Parameters != null)
            {
                var requestHeaders = response.Request.Parameters.Where(p => p.Type == ParameterType.HttpHeader);
                if (requestHeaders.Any())
                {
                    Header = string.Join("\n", requestHeaders.Select(h => $"{h.Name}: {h.ToRequestString()}"));
                }

                var requestBody = response.Request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
                var bodyByteArray = requestBody?.Value as byte[];
                if (bodyByteArray != null)
                {
                    Body = System.Text.Encoding.UTF8.GetString(bodyByteArray);
                }
            }
        }

        public string Url { get; set; }
        public string Method { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
    }
}