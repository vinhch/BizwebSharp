using System.Runtime.Serialization;
using BizwebSharp.Converters;
using Newtonsoft.Json;

namespace BizwebSharp.Enums
{
    [JsonConverter(typeof(NullableEnumConverter<CollectionTypes>))]
    public enum CollectionTypes
    {
        [EnumMember(Value = "custom")] Custom,
        [EnumMember(Value = "smart")] Smart
    }
}