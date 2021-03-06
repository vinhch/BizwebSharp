﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    ///     The class representing a product image.
    /// </summary>
    public class ProductImage : BaseEntityWithTimeline
    {
        /// <summary>
        ///     The id of the product associated with the image.
        /// </summary>
        [JsonProperty("product_id")]
        public long? ProductId { get; set; }

        /// <summary>
        ///     The order of the product image in the list. The first product image is at position 1 and is the "main" image for
        ///     the product.
        /// </summary>
        [JsonProperty("position")]
        public int? Position { get; set; }

        /// <summary>
        ///     Specifies the location of the product image.
        /// </summary>
        [JsonProperty("src")]
        public string Src { get; set; }

        /// <summary>
        /// Specifies the file name of the image when creating a <see cref="ProductImage"/>, where it's then converted into the <see cref="Src"/> path
        /// </summary>
        [JsonProperty("filename", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Filename { get; set; }

        /// <summary>
        ///     A base64 image attachment. Only used when creating a <see cref="ProductImage" />, where it's then converted into
        ///     the <see cref="Src" />.
        /// </summary>
        [JsonProperty("base64")]
        public string Base64 { get; set; }

        /// <summary>
        ///     An array of variant ids associated with the image.
        /// </summary>
        [JsonProperty("variant_ids")]
        public IEnumerable<long> VariantIds { get; set; }

        /// <summary>
        ///     Attaches additional information to a shop's resources.
        /// </summary>
        [JsonProperty("metafields")]
        public IEnumerable<MetaField> Metafields { get; set; }
    }
}