using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// The class representing Bizweb blog article.
    /// </summary>
    public class Article : BaseEntityCanPublishable
    {
        /// <summary>
        /// A unique numeric identifier for the blog containing the article.
        /// </summary>
        [JsonProperty("blog_id")]
        public long? BlogId { get; set; }

        /// <summary>
        /// The title of the article.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// A human-friendly unique string for this resource automatically generated from its title.
        /// It is used in resource's URL.
        /// </summary>
        [JsonProperty("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// A unique numeric identifier for the author of the article.
        /// </summary>
        [JsonProperty("user_id")]
        public long? UserId { get; set; }

        /// <summary>
        /// The text of the body of the article, complete with HTML markup.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// The text of the summary of the article, complete with HTML markup.
        /// </summary>
        [JsonProperty("summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Tags are additional short descriptors formatted as a string of comma-separated values.
        /// For example, if an article has three tags: tag1, tag2, tag3.
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; }

        /// <summary>
        /// States the name of the template this resource is using if it is using an alternate template.
        /// If this resource is using the default template, the value returned is null.
        /// </summary>
        [JsonProperty("template_layout")]
        public string TemplateLayout { get; set; }

        /// <summary>
        /// The name of the author of this article
        /// </summary>
        [JsonProperty("author")]
        public string Author { get; set; }

        /// <summary>
        /// The article image.
        /// </summary>
        [JsonProperty("image")]
        public Image Image { get; set; }
    }
}