using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class EventListOption : ListOption
    {
        /// <summary>
        /// Only show events specified in filter (comma , separated). A full list of events can be found at https://help.shopify.com/api/reference/event
        /// </summary>
        [JsonProperty("filter")]
        public string Filters { get; set; }

        /// <summary>
        /// Only show events of a certain kind (comma , separated). A full list of events can be found at https://help.shopify.com/api/reference/event
        /// </summary>
        [JsonProperty("verb")]
        public string Verbs { get; set; }
    }
}
