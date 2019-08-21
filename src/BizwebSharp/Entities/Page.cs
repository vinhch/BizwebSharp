using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// The class representing Bizweb page.
    /// </summary>
    public class Page : BaseEntityCanPublishable
    {
        /// <summary>
        /// The name of the page.
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
        /// The content of the page, complete with HTML formatting.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// The text of the description of the page.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

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

        /// <summary>
        /// The author name of this page
        /// </summary>
        [JsonProperty("author")]
        public string Author { get; set; }
    }
}