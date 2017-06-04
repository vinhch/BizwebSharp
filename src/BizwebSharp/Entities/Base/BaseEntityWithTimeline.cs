using System;
using Newtonsoft.Json;

namespace BizwebSharp
{
    public abstract class BaseEntityWithTimeline : BaseEntity
    {
        /// <summary>
        ///     The date and time when the entity was created. The API returns this value in ISO 8601 format.
        /// </summary>
        [JsonProperty("created_on", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? CreatedOn { get; set; }

        /// <summary>
        ///     The date and time when the entity was last modified. The API returns this value in ISO 8601 format.
        /// </summary>
        [JsonProperty("modified_on", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? ModifiedOn { get; set; }
    }
}