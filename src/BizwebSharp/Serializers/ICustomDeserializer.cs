using RestSharp.Portable;

namespace BizwebSharp.Serializers
{
    public interface ICustomDeserializer : IDeserializer
    {
        string RootElement { get; set; }
    }
}