using Newtonsoft.Json;

namespace BizwebSharp
{
    public class SmartCollectionRule : BaseEntity
    {
        /// <summary>
        ///     The relationship between the column choice, and the condition.
        /// </summary>
        [JsonProperty("relation")]
        public string Relation { get; set; }

        /// <summary>
        ///     Select products for a collection using a condition. Conditions are either strings or numbers, depending on the
        ///     relation.
        /// </summary>
        [JsonProperty("condition")]
        public string Condition { get; set; }

        /// <summary>
        ///     The properties of a product that can be used to to populate a collection.
        /// </summary>
        [JsonProperty("column")]
        public string Column { get; set; }

        [JsonProperty("collection_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? CollectionId { get; set; }
    }
}