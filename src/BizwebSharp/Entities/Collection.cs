﻿using System.Collections.Generic;
using BizwebSharp.Enums;
using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class Collection : BaseEntityCanPublishable
    {
        [JsonProperty("type")]
        public CollectionTypes Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

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
        public Image Image { get; set; }

        [JsonProperty("rules")]
        public IEnumerable<SmartCollectionRule> Rules { get; set; }
    }
}