﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace BizwebSharp
{
    public class Fulfillment : BaseEntityWithTimeline
    {
        [JsonProperty("variant_inventory_management")]
        public string VariantInventoryManagement { get; set; }

        /// <summary>
        /// A historical record of each item in the fulfillment.
        /// </summary>
        [JsonProperty("line_items")]
        public IEnumerable<LineItem> LineItems { get; set; }

        /// <summary>
        /// The unique numeric identifier for the order.
        /// </summary>
        [JsonProperty("order_id")]
        public long? OrderId { get; set; }

        /// <summary>
        /// A textfield with information about the receipt.
        /// </summary>
        [JsonProperty("receipt")]
        public object Receipt { get; set; }

        /// <summary>
        /// The status of the fulfillment. Valid values are 'pending', 'open', 'success', 'cancelled',
        /// 'error' and 'failure'.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// The name of the shipping company.
        /// </summary>
        [JsonProperty("tracking_company")]
        public string TrackingCompany { get; set; }

        /// <summary>
        /// The shipping number, provided by the shipping company. If multiple tracking numbers
        /// exist (<see cref="TrackingNumbers"/>), returns the first number.
        /// </summary>
        [JsonProperty("tracking_number")]
        public string TrackingNumber { get; set; }

        /// <summary>
        /// A list of shipping numbers, provided by the shipping company. May be null.
        /// </summary>
        [JsonProperty("tracking_numbers")]
        public IEnumerable<string> TrackingNumbers { get; set; }

        /// <summary>
        /// The tracking url, provided by the shipping company. May be null. If multiple tracking URLs
        /// exist (<see cref="TrackingUrls"/>), returns the first URL.
        /// </summary>
        [JsonProperty("tracking_url")]
        public string TrackingUrl { get; set; }

        /// <summary>
        /// An array of one or more tracking urls, provided by the shipping company. May be null.
        /// </summary>
        [JsonProperty("tracking_urls")]
        public IEnumerable<string> TrackingUrls { get; set; }
    }
}