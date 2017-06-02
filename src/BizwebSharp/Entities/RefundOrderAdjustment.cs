using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class RefundOrderAdjustment : BaseEntity
    {
        /// <summary>
        /// The unique identifier of the order
        /// </summary>
        [JsonProperty("order_id")]
        public long? OrderId { get; set; }

        /// <summary>
        /// The unique identifier of the refund
        /// </summary>
        [JsonProperty("refund_id")]
        public long? RefundId { get; set; }

        /// <summary>
        /// The amount refunded (it is negative and does not include tax).
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The tax amount refunded (negative).
        /// </summary>
        [JsonProperty("tax_amount")]
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// The type of adjustment. Values include "refund_discrepancy", "shipping_refund"
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Reason for the refund
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}