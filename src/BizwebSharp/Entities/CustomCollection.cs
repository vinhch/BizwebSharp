﻿using Newtonsoft.Json;

namespace BizwebSharp
{
    public class CustomCollection : BaseEntityCanPublishable
    {
        /// <summary>
        /// The name of the collection. Limit of 255 characters.
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
        /// The description of the collection, complete with HTML markup.
        /// Many templates display this on their collection page.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// The content will be shown in meta title html tag.
        /// Many templates display this on their collection page.
        /// </summary>
        [JsonProperty("meta_title")]
        public string MetaTitle { get; set; }

        /// <summary>
        /// The content will be shown in meta description html tag.
        /// Many templates display this on their collection page.
        /// </summary>
        [JsonProperty("meta_description")]
        public string MetaDescription { get; set; }

        /// <summary>
        /// States the name of the template this resource is using if it is using an alternate template.
        /// If this resource is using the default template, the value returned is null.
        /// </summary>
        [JsonProperty("template_layout")]
        public string TemplateLayout { get; set; }

        /// <summary>
        /// The order in which products in the collection appear.
        /// </summary>
        [JsonProperty("sort_order")]
        public string SortOrder { get; set; }

        /// <summary>
        /// If false, products must match all of the rules to be included in the collection. If true, products can only match one of the rules.
        /// </summary>
        [JsonProperty("disjunctive")]
        public bool? Disjunctive { get; set; }

        /// <summary>
        /// The number of products in this collection.
        /// </summary>
        [JsonProperty("products_count")]
        public bool? ProductsCount { get; set; }

        /// <summary>
        /// The collection image.
        /// </summary>
        [JsonProperty("image")]
        public CustomCollectionImage Image { get; set; }
    }
}