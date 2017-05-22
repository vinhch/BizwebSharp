﻿using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class CollectOption : ListOptions
    {
        /// <summary>
        /// An optional product id to retrieve.
        /// </summary>
        [JsonProperty("product_id")]
        public long? ProductId { get; set; }

        /// <summary>
        /// An optional collection id to retrieve.
        /// </summary>
        [JsonProperty("collection_id")]
        public long? CollectionId { get; set; }
    }
}
