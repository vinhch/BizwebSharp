using BizwebSharp.Enums;
using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class Theme : BaseEntityWithTimeline
    {
        /// <summary>
        /// The name of the theme.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Specifies how the theme is being used within the shop. Known values are 'main', 'mobile' and 'unpublished'.
        /// </summary>
        [JsonProperty("role")]
        public ThemeRole Role { get; set; }

        /// <summary>
        /// Indicates if the theme can currently be previewed.
        /// </summary>
        [JsonProperty("previewable")]
        public bool Previewable { get; set; }

        /// <summary>
        /// Indicates if files are still being copied into place for this theme.
        /// </summary>
        [JsonProperty("processing")]
        public bool Processing { get; set; }
    }
}