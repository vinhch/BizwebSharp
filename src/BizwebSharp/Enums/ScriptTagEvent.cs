using System.Runtime.Serialization;
using BizwebSharp.Converters;
using Newtonsoft.Json;

namespace BizwebSharp.Enums
{
    [JsonConverter(typeof(NullableEnumConverter<ScriptTagEvent>))]
    public enum ScriptTagEvent
    {
        /// <summary>
        /// The script tag is triggered when the DOM fires the "Onload" event.
        /// </summary>
        [EnumMember(Value = "onload")]
        Onload
    }
}
