using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// The class representing Bizweb image.
    /// </summary>
    public class Image : BaseEntityWithTimeline
    {
        /// <summary>
        /// Source URL that specifies the location of the image.
        /// </summary>
        [JsonProperty("src")]
        public string Src { get; set; }

        /// <summary>
        /// An image attached to a shop's theme returned as Base64-encoded binary data.
        /// </summary>
        [JsonProperty("base64")]
        public string Base64 { get; set; }

        /// <summary>
        /// The image name
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Image file extension
        /// </summary>
        [JsonProperty("extension", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Extension { get; set; }

        /// <summary>
        /// Image content type
        /// </summary>
        [JsonProperty("content_type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ContentType { get; set; }

        /// <summary>
        /// Image file size
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Size { get; set; }
    }
}