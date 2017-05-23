using BizwebSharp.Infrastructure;
using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class BlogOption : ListOption
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }
    }
}