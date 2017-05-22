using BizwebSharp.Enums;
using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class WebhookOption : ListOptions
    {
        /// <summary>
        /// An optional filter for the address property. When used, the method will only return webhooks with the given address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// An optional filter for the topic property. When used, the method will only return webhooks with the given topic. A full list of topics can be found at https://help.shopify.com/api/reference/webhook.
        /// </summary>
        [JsonProperty("topic")]
        public WebhookTopic? Topic { get; set; }
    }
}
