using System;
using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class Page : BaseEntityWithTimeline
    {
        [JsonProperty("published_on", DefaultValueHandling = DefaultValueHandling.Include,
             NullValueHandling = NullValueHandling.Include)]
        public DateTime? PublishedOn { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("meta_title")]
        public string MetaTitle { get; set; }

        [JsonProperty("meta_description")]
        public string MetaDescription { get; set; }

        [JsonProperty("template_layout")]
        public string TemplateLayout { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }
    }
}