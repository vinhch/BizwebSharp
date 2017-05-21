using System.Runtime.Serialization;
using BizwebSharp.Converters;
using Newtonsoft.Json;

namespace BizwebSharp.Enums
{
    [JsonConverter(typeof(NullableEnumConverter<TransactionKind>))]
    public enum TransactionKind
    {
        [EnumMember(Value = "authorization")]
        Authorization,

        [EnumMember(Value = "capture")]
        Capture,

        [EnumMember(Value = "sale")]
        Sale,

        [EnumMember(Value = "void")]
        Void,

        [EnumMember(Value = "refund")]
        Refund
    }
}
