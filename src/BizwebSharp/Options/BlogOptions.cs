using BizwebSharp.Infrastructure;
using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class BlogOptions : Parameterizable
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("since_id")]
        public long? SinceId { get; set; }

        [JsonProperty("fields")]
        public string Fields { get; set; }
    }
}