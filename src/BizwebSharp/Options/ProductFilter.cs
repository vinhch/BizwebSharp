using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class ProductOptions : PublishableListOptions
    {
        /// <summary>
        ///     Filter by product title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; } = null;

        /// <summary>
        ///     Filter by product vendor.
        /// </summary>
        [JsonProperty("vendor")]
        public string Vendor { get; set; } = null;

        /// <summary>
        ///     Filter by product handle.
        /// </summary>
        [JsonProperty("handle")]
        public string Handle { get; set; } = null;

        /// <summary>
        ///     Filter by product type.
        /// </summary>
        [JsonProperty("product_type")]
        public string ProductType { get; set; } = null;

        /// <summary>
        ///     Filter by collection id.
        /// </summary>
        [JsonProperty("collection_id")]
        public long? CollectionId { get; set; } = null;
    }
}