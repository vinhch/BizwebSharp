using Newtonsoft.Json;

namespace BizwebSharp.Infrastructure
{
    /// <summary>
    /// Contains JSON serialization settings and methods used by the rest of the ShopifySharp package.
    /// </summary>
    internal static class Serializer
    {
        internal static JsonSerializerSettings SerializeSettings { get; } = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        internal static JsonSerializerSettings DeserializeSettings { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateParseHandling = DateParseHandling.DateTimeOffset
        };
    }
}
