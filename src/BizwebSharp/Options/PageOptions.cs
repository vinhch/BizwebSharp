using Newtonsoft.Json;

namespace BizwebSharp.Options
{
    public class PageOptions : PublishableListOptions
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }
    }
}