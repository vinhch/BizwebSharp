using Newtonsoft.Json;
using System;

namespace BizwebSharp.Options
{
    public class PriceRuleOption : ListOption
    {
        /// <summary>
        /// Show price rules starting after date.
        /// </summary>
        [JsonProperty("starts_on_min")]
        public DateTimeOffset? StartsOnMin { get; set; }

        /// <summary>
        /// Show price rules starting before date.
        /// </summary>
        [JsonProperty("starts_on_max")]
        public DateTimeOffset? StartsOnMax { get; set; }

        /// <summary>
        /// Show price rules ending after date.
        /// </summary>
        [JsonProperty("ends_on_min")]
        public DateTimeOffset? EndsOnMin { get; set; }

        /// <summary>
        /// Show price rules ending before date.
        /// </summary>
        [JsonProperty("ends_on_max")]
        public DateTimeOffset? EndsOnMax { get; set; }

        /// <summary>
        /// Show price rules with times used.
        /// </summary>
        [JsonProperty("times_used")]
        public int? TimesUsed { get; set; }
    }
}
