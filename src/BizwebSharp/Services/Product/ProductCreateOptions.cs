using BizwebSharp.Infrastructure;
using Newtonsoft.Json;

namespace BizwebSharp.Services
{
    public class ProductCreateOptions : Parameterizable
    {
        [JsonProperty("published")]
        public bool? Published { get; set; }
    }
}