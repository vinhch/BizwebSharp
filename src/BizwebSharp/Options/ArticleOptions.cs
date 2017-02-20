using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class ArticleOptions : PublishableListOptions
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("blog_id")]
        public long? BlogId { get; set; }
    }
}