using BizwebSharp.Infrastructure;
using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class PublishedOnCreateOption : Parameterizable
    {
        [JsonProperty("published")]
        public bool? Published { get; set; }
    }
}
