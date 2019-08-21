using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// Base class for Bizweb entities.
    /// All Bizweb entities should inherit from this class so that we can
    /// apply share properties and methods like Id...
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        ///     The object's unique id.
        /// </summary>
        /// <remarks>
        ///     Some object ids are longer than the max int32 value. Using long? instead.
        /// </remarks>
        [JsonProperty("id")]
        public long? Id { get; set; }
    }
}