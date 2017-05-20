using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class ArticleOption : PublishableListOptions
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("blog_id")]
        public long? BlogId { get; set; }
    }
}