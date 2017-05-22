using System.Runtime.Serialization;
using BizwebSharp.Converters;
using Newtonsoft.Json;

namespace BizwebSharp.Enums
{
    [JsonConverter(typeof(NullableEnumConverter<OrderFinancialStatus>))]
    public enum OrderFinancialStatus
    {
        [EnumMember(Value = "pending")]
        Pending,

        [EnumMember(Value = "authorized")]
        Authorized,

        [EnumMember(Value = "partially_paid")]
        PartiallyPaid,

        [EnumMember(Value = "paid")]
        Paid,

        [EnumMember(Value = "partially_refunded")]
        PartiallyRefunded,

        [EnumMember(Value = "refunded")]
        Refunded,

        [EnumMember(Value = "voided")]
        Voided
    }
}
