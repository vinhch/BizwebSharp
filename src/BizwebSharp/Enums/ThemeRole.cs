using System.Runtime.Serialization;
using BizwebSharp.Converters;
using Newtonsoft.Json;

namespace BizwebSharp.Enums
{
    /// <summary>
    /// Specifies how a theme is being used within the shop.
    /// </summary>
    [JsonConverter(typeof(NullableEnumConverter<ThemeRole>))]
    public enum ThemeRole
    {
        /// <summary>
        /// This theme can be seen by customers when visiting the shop in a desktop browser.
        /// </summary>
        [EnumMember(Value = "main")]
        Main,

        /// <summary>
        /// This theme can be seen by customers when visiting the shop in a mobile browser.
        /// </summary>
        [EnumMember(Value = "mobile")]
        Mobile,

        /// <summary>
        /// This theme cannot currently be seen by customers.
        /// </summary>
        [EnumMember(Value = "unpublished")]
        Unpublished
    }
}
