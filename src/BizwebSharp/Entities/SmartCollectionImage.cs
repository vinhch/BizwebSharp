using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class SmartCollectionImage : BaseEntityWithTimeline
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("base64")]
        public string Base64 { get; set; }

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("extension", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Extension { get; set; }

        [JsonProperty("content_type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ContentType { get; set; }

        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long Size { get; set; }
    }
}