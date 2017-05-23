using BizwebSharp.Enums;
using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class OrderOption : ListOption
    {
        /// <summary>
        /// The status of orders to retrieve. Known values are "open", "closed", "cancelled" and "any".
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// The financial status of orders to retrieve. Known values are "authorized", "paid", "pending", "partially_paid", "partially_refunded", "refunded" and "voided".
        /// </summary>
        [JsonProperty("financial_status")]
        public OrderFinancialStatus FinancialStatus { get; set; }

        /// <summary>
        /// The fulfillment status of orders to retrieve. Leave this null to retrieve orders with any fulfillment status.
        /// </summary>
        [JsonProperty("fulfillment_status")]
        public string FulfillmentStatus { get; set; }
    }
}
