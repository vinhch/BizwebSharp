using System;
using RestSharp.Portable;

namespace BizwebSharp.Infrastructure
{
    internal class CustomRestRequest : RestRequest, ICustomRestRequest
    {
        public CustomRestRequest()
        {
        }

        public CustomRestRequest(string resource) : base(resource)
        {
        }

        public CustomRestRequest(Uri resource) : base(resource)
        {
        }

        public CustomRestRequest(Method method) : base(method)
        {
        }

        public CustomRestRequest(Uri resource, Method method) : base(resource, method)
        {
        }

        public CustomRestRequest(string resource, Method method) : base(resource, method)
        {
        }

        public string RootElement { get; set; }
    }
}