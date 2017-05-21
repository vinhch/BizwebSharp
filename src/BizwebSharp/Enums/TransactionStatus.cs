using System.Runtime.Serialization;
using BizwebSharp.Converters;
using Newtonsoft.Json;

namespace BizwebSharp.Enums
{
    [JsonConverter(typeof(NullableEnumConverter<TransactionStatus>))]
    public enum TransactionStatus
    {
        [EnumMember(Value = "pending")]
        Pending,

        [EnumMember(Value = "failure")]
        Failure,

        [EnumMember(Value = "success ")]
        Success,

        [EnumMember(Value = "error")]
        Error
    }
}
