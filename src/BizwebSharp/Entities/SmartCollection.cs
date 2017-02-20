using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class SmartCollection : BaseEntityWithTimeline
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("published_on", DefaultValueHandling = DefaultValueHandling.Include,
             NullValueHandling = NullValueHandling.Include)]
        public DateTime? PublishedOn { get; set; }

        [JsonProperty("meta_title")]
        public string MetaTitle { get; set; }

        [JsonProperty("meta_description")]
        public string MetaDescription { get; set; }

        [JsonProperty("template_layout")]
        public string TemplateLayout { get; set; }

        [JsonProperty("sort_order")]
        public string SortOrder { get; set; }

        [JsonProperty("disjunctive")]
        public bool Disjunctive { get; set; }

        [JsonProperty("products_count")]
        public bool ProductsCount { get; set; }

        [JsonProperty("image")]
        public SmartCollectionImage Image { get; set; }

        [JsonProperty("rules")]
        public IList<SmartCollectionRules> Rules { get; set; }
    }
}