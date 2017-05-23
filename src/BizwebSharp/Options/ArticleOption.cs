using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class ArticleOption : PublishableListOption
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("blog_id")]
        public long? BlogId { get; set; }
    }
}