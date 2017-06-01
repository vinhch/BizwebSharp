using System.Runtime.Serialization;
using BizwebSharp.Converters;
using Newtonsoft.Json;

namespace BizwebSharp.Enums
{
    [JsonConverter(typeof(NullableEnumConverter<BlogCommentable>))]
    public enum BlogCommentable
    {
        [EnumMember(Value = "yes")]
        Yes,
        [EnumMember(Value = "no")]
        No,
        [EnumMember(Value = "moderate")]
        Moderate,
    }
}
