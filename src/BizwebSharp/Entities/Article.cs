using Newtonsoft.Json;

namespace BizwebSharp
{
    public class Article : BaseEntityCanPublishable
    {
        [JsonProperty("blog_id")]
        public long? BlogId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("user_id")]
        public long? UserId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("template_layout")]
        public string TemplateLayout { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }
    }
}