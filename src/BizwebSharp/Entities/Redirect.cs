﻿using Newtonsoft.Json;

namespace BizwebSharp
{
    /// <summary>
    /// The class representing Bizweb redirect.
    /// </summary>
    public class Redirect : BaseEntity
    {
        /// <summary>
        /// The "before" path to be redirected. When the user navigates to this path, they will be redirected to the path specified by target.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// The "after" path or URL to be redirected to. This property can be set to any path on the shop's site, or any URL, even one on a
        /// completely different domain.
        /// </summary>
        [JsonProperty("target")]
        public string Target { get; set; }
    }
}
