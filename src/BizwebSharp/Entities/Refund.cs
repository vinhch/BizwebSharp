﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// The class representing Bizweb refund.
    /// </summary>
    public class Refund : BaseEntityWithTimeline
    {
        /// <summary>
        /// The list of <see cref="RefundOrderAdjustment"/> objects
        /// </summary>
        [JsonProperty("order_adjustments")]
        public IEnumerable<RefundOrderAdjustment> OrderAdjustments { get; set; }

        /// <summary>
        /// The date and time when the refund was imported.
        /// </summary>
        /// <remarks>
        /// This value can be set to dates in the past when importing from other systems. If no value is provided, it will be auto-generated.
        /// </remarks>
        [JsonProperty("processed_at")]
        public DateTimeOffset? ProcessedAt { get; set; }

        /// <summary>
        /// The optional note attached to a refund.
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; set; }

        /// <summary>
        /// The list of <see cref="RefundLineItem"/> objects
        /// </summary>
        [JsonProperty("refund_line_items")]
        public IEnumerable<RefundLineItem> RefundLineItems { get; set; }

        /// <summary>
        /// Whether or not the line items were added back to the store inventory.
        /// </summary>
        [JsonProperty("restock")]
        public bool? Restock { get; set; }

        /// <summary>
        /// The list of <see cref="Transaction"/> objects
        /// </summary>
        [JsonProperty("transactions")]
        public IEnumerable<Transaction> Transactions { get; set; }

        /// <summary>
        /// The unique identifier of the user who performed the refund.
        /// </summary>
        [JsonProperty("user_id")]
        public long? UserId { get; set; }
    }
}