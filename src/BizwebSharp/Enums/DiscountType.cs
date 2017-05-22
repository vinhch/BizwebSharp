using System.Runtime.Serialization;
using BizwebSharp.Converters;
using Newtonsoft.Json;

namespace BizwebSharp.Enums
{
    [JsonConverter(typeof(NullableEnumConverter<DiscountType>))]
    public enum DiscountType
    {
        [EnumMember(Value = "percentage")]
        Percentage,

        [EnumMember(Value = "shipping")]
        Shipping,

        [EnumMember(Value = "fixed_amount")]
        FixedAmount,

        [EnumMember(Value = "none")]
        None,
    }
}
