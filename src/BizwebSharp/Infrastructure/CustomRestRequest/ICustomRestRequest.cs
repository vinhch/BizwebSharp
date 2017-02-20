using RestSharp.Portable;

namespace BizwebSharp.Infrastructure
{
    public interface ICustomRestRequest : IRestRequest
    {
        string RootElement { get; set; }
    }
}