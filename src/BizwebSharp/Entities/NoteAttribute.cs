using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class NoteAttribute
    {
        /// <summary>
        /// The name of the note attribute.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The value of the note attribute.
        /// </summary>
        [JsonProperty("value")]
        public object Value { get; set; }
    }
}