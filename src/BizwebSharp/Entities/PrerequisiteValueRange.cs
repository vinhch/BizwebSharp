using Newtonsoft.Json;

namespace BizwebSharp
{
    public class PrerequisiteValueRange
    {
    }

    public class PrerequisiteSubtotalRange
    {
        [JsonProperty("greater_than_or_equal_to")]
        public decimal? GreaterThanOrEqualTo { get; set; }
    }

    public class PrerequisiteShippingPriceRange
    {
        [JsonProperty("less_than_or_equal_to")]
        public decimal? LessThanOrEqualTo { get; set; }
    }
}