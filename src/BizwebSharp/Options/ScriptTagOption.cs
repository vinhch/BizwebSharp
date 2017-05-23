using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class ScriptTagOption : ListOption
    {
        /// <summary>
        /// Returns only those <see cref="ScriptTag"/>s with the given <see cref="ScriptTag.Src"/> value.
        /// </summary>
        [JsonProperty("src")]
        public string Src { get; set; }
    }
}
