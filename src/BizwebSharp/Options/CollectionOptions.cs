using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class CollectionOptions : PublishableListOptions
    {
        /// <summary>
        ///     Show smart collections with given title
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Show smart collections that includes given product
        /// </summary>
        [JsonProperty("product_id")]
        public long? ProductId { get; set; }

        /// <summary>
        ///     Filter by smart collection handle
        /// </summary>
        [JsonProperty("alias")]
        public string Alias { get; set; }
    }
}