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
                    Header = string.Join("\n", requestHeaders.Select(h => $"{h.Name}: {h.Value.ToString()}"));

                var requestBody = response.Request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
                Body = requestBody?.Value.ToString();
            }
        }

        public string Url { get; set; }
        public string Method { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
    }
}