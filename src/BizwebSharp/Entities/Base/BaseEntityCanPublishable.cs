using System;
using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// Base class for Bizweb entities.
    /// Bizweb entities that has timeline and publishable state should inherit from this class so that we can
    /// apply share properties and methods like Id, CreatedOn, ModifiedOn, PublishedOn, Published...
    /// </summary>
    public abstract class BaseEntityCanPublishable : BaseEntityWithTimeline
    {
        /// <summary>
        /// The date and time when the resource was published.
        /// </summary>
        [JsonProperty("published_on", DefaultValueHandling = DefaultValueHandling.Include,
             NullValueHandling = NullValueHandling.Include)]
        public DateTimeOffset? PublishedOn { get; set; }

        /// <summary>
        /// States whether or not the resource is visible.
        /// </summary>
        [JsonProperty("published")]
        public bool? Published { get; set; }

        /// <summary>
        /// The sales channels in which the resource is visible.
        /// </summary>
        [JsonProperty("published_scope")]
        public string PublishedScope { get; set; }
    }
}
