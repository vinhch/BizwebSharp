using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// The class representing Bizweb asset.
    /// Assets do not have ids, but rather keys, and are associated with specific themes.
    /// </summary>
    public class Asset : BaseEntityWithTimeline
    {
        [JsonProperty("base64")]
        public string Base64 { get; set; }

        /// <summary>
        /// An asset attached to a store's theme.
        /// </summary>
        [JsonProperty("attachment")]
        public string Attachment { get; set; }

        /// <summary>
        /// MIME representation of the content, consisting of the type and subtype of the asset,
        /// e.g. "image/gif"
        /// </summary>
        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// The path to the asset within a shop, prefixed with the asset's 'bucket' type,
        ///  e.g. 'templates/index.liquid' or 'assets/bg-body.gif'.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// The public facing URL of the asset.
        /// </summary>
        [JsonProperty("public_url")]
        public string PublicUrl { get; set; }

        /// <summary>
        /// The asset size in bytes.
        /// </summary>
        [JsonProperty("size")]
        public long? Size { get; set; }

        /// <summary>
        /// When set in an asset and used in <see cref="AssetService.CreateOrUpdateAsync(long, Asset)"/>,
        /// a new asset will be created and copied from an asset with the key matching this source key.
        /// </summary>
        [JsonProperty("source_key")]
        public string SourceKey { get; set; }

        /// <summary>
        /// Specifies the location of an asset.
        /// </summary>
        [JsonProperty("src")]
        public string Src { get; set; }

        /// <summary>
        /// A unique numeric identifier for the theme.
        /// </summary>
        [JsonProperty("theme_id")]
        public long? ThemeId { get; set; }

        /// <summary>
        /// The asset that you are adding.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
