using BizwebSharp.Infrastructure;
using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class BlogOption : ListOptions
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }
    }
}