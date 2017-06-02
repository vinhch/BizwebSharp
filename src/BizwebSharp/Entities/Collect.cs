using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class Collect : BaseEntityWithTimeline
    {
        /// <summary>
        /// The id of the custom collection containing the product.
        /// </summary>
        [JsonProperty("collection_id")]
        public long? CollectionId { get; set; }

        /// <summary>
        /// The unique numeric identifier for the product in the custom collection.
        /// </summary>
        [JsonProperty("product_id")]
        public long? ProductId { get; set; }

        /// <summary>
        /// States whether or not the collect is featured.
        /// </summary>
        [JsonProperty("featured")]
        public bool Featured { get; set; }

        /// <summary>
        /// A number specifying the manually sorted position of this product in a custom collection. The first position is 1. This value only applies when the custom collection is viewed using the Manual sort order.
        /// </summary>
        [JsonProperty("position")]
        public int? Position { get; set; }

        /// <summary>
        /// This is the same value as position but padded with leading zeroes to make it alphanumeric-sortable.
        /// </summary>
        [JsonProperty("sort_value")]
        public string SortValue { get; set; }
    }
}
