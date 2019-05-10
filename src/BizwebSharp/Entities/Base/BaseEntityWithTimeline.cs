using System;
using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// Base class for Bizweb entities.
    /// Bizweb entities that has timeline state should inherit from this class so that we can
    /// apply share properties and methods like Id, CreatedOn, ModifiedOn...
    /// </summary>
    public abstract class BaseEntityWithTimeline : BaseEntity
    {
        /// <summary>
        ///     The date and time when the resource was created. The API returns this value in ISO 8601 format.
        /// </summary>
        [JsonProperty("created_on", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? CreatedOn { get; set; }

        /// <summary>
        ///     The date and time when the resource was last modified. The API returns this value in ISO 8601 format.
        /// </summary>
        [JsonProperty("modified_on", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? ModifiedOn { get; set; }
    }
}