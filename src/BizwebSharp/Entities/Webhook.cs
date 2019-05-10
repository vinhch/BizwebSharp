using System.Collections.Generic;
using BizwebSharp.Enums;
using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// The class representing Bizweb Webhook.
    /// </summary>
    public class Webhook : BaseEntityWithTimeline
    {
        /// <summary>
        /// The URL where the webhook should send the POST request when the event occurs.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// An optional array of fields which should be included in webhooks.
        /// </summary>
        [JsonProperty("fields")]
        public IEnumerable<string> Fields { get; set; }

        /// <summary>
        /// The format in which the webhook should send the data. Valid values are json and xml.
        /// </summary>
        [JsonProperty("format")]
        public string Format { get; set; }

        /// <summary>
        /// An optional array of namespaces for metafields that should be included in webhooks.
        /// </summary>
        [JsonProperty("metafield_namespaces")]
        public IEnumerable<string> MetafieldNamespaces { get; set; }

        /// <summary>
        /// The event that will trigger the webhook, e.g. 'orders/create' or 'app/uninstalled'. A full list of webhook topics can be found at https://help.shopify.com/api/reference/webhook.
        /// </summary>
        [JsonProperty("topic")]
        public WebhookTopic Topic { get; set; }
    }
}
