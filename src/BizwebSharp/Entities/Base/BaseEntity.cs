using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public abstract class BaseEntity
    {
        /// <summary>
        ///     The object's unique id.
        /// </summary>
        /// <remarks>
        ///     Some object ids are longer than the max int32 value. Using long instead.
        /// </remarks>
        [JsonProperty("id")]
        public long? Id { get; set; }
    }
}