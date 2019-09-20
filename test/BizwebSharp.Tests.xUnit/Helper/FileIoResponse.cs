using Newtonsoft.Json;

namespace BizwebSharp.Tests.xUnit
{
    public class FileIoResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("expiry")]
        public string Expiry { get; set; }
    }
}
