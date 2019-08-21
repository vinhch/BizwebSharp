using BizwebSharp.Enums;
using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// The class representing Bizweb blog.
    /// </summary>
    public class Blog : BaseEntityWithTimeline
    {
        /// <summary>
        /// The name (title) of the blog.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// A human-friendly unique string for this resource automatically generated from its title.
        /// It is used in resource's URL.
        /// </summary>
        [JsonProperty("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// The text of the description of the blog.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Indicates whether readers can post comments to the blog and if comments are moderated or not. Possible values are:
        /// "no" (default): Readers cannot post comments to blog articles.
        /// "moderate": Readers can post comments to blog articles, but comments must be moderated before they appear.
        /// "yes": Readers can post comments to blog articles without moderation.
        /// </summary>
        [JsonProperty("commentable")]
        public BlogCommentable Commentable { get; set; }

        [JsonProperty("meta_title")]
        public string MetaTitle { get; set; }

        [JsonProperty("meta_description")]
        public string MetaDescription { get; set; }

        /// <summary>
        /// States the name of the template this resource is using if it is using an alternate template.
        /// If this resource is using the default template, the value returned is null.
        /// </summary>
        [JsonProperty("template_layout")]
        public string TemplateLayout { get; set; }
    }
}