using BizwebSharp.Enums;
using Newtonsoft.Json;

namespace BizwebSharp
{
    public class ScriptTag : BaseEntityWithTimeline
    {
        ///// <summary>
        ///// Where the script tag should be included on the store. Known values are 'online_store', 'order_status' or 'all'. Defaults to 'all'.
        ///// </summary>
        //[JsonProperty("display_scope")]
        //public string DisplayScope { get; set; }

        /// <summary>
        /// DOM event which triggers the loading of the script. The only known value is 'onload'.
        /// </summary>
        [JsonProperty("event")]
        public ScriptTagEvent Event { get; set; }

        /// <summary>
        /// Specifies the location of the ScriptTag.
        /// </summary>
        [JsonProperty("src")]
        public string Src { get; set; }
    }
}
