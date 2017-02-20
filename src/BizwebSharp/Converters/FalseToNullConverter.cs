using System;
using Newtonsoft.Json;

namespace BizwebSharp.Converters
{
    internal class FalseToNullConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (string.IsNullOrEmpty(reader.Value?.ToString()))
                return false;

            bool output;
            if (bool.TryParse(reader.Value.ToString(), out output))
                return output;

            throw new JsonReaderException($"Cannot convert given JSON value with {nameof(FalseToNullConverter)}.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                if (bool.Parse(value.ToString()))
                    writer.WriteValue(true);
                else
                    writer.WriteNull();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(string)) || (objectType == typeof(bool)) || (objectType == typeof(Nullable));
        }
    }
}