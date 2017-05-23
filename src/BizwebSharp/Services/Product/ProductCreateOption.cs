using BizwebSharp.Infrastructure;
using Newtonsoft.Json;

namespace BizwebSharp.Services
{
    public class ProductCreateOption : Parameterizable
    {
        [JsonProperty("published")]
        public bool? Published { get; set; }
    }
}