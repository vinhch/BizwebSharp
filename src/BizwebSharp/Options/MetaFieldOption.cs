using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class MetaFieldOption : ListOptions
    {
        /// <summary>
        /// Filter by namespace.
        /// </summary>
        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        /// <summary>
        /// Filter by key value.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Filter by value_type.
        /// </summary>
        [JsonProperty("value_type")]
        public string ValueType { get; set; }
    }
}
