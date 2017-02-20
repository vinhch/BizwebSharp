using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class Blog : BaseEntityWithTimeline
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("commentable")]
        public string Commentable { get; set; }

        [JsonProperty("meta_title")]
        public string MetaTitle { get; set; }

        [JsonProperty("meta_description")]
        public string MetaDescription { get; set; }

        [JsonProperty("template_layout")]
        public string TemplateLayout { get; set; }
    }
}