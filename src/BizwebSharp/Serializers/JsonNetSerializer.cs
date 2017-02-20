using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp.Portable;

namespace BizwebSharp.Serializers
{
    public class JsonNetSerializer : ICustomDeserializer, ISerializer
    {
        private static readonly Encoding _encoding = new UTF8Encoding(false);

        //public string DateFormat { get; set; }

        public JsonNetSerializer()
        {
            ContentType = $"application/json; charset={_encoding.WebName}";
        }

        public string RootElement { get; set; }

        public T Deserialize<T>(IRestResponse response)
        {
            var output = Activator.CreateInstance<T>();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            //if (!string.IsNullOrEmpty(DateFormat))
            //{
            //    settings.DateFormatString = DateFormat;
            //}

            if (string.IsNullOrEmpty(RootElement))
            {
                output = JsonConvert.DeserializeObject<T>(response.Content, settings);
            }
            else
            {
                var data = JsonConvert.DeserializeObject(response.Content, settings) as JToken;

                if (data[RootElement] != null)
                    output = data[RootElement].ToObject<T>();
            }

            return output;
        }

        public string ContentType { get; set; }

        public byte[] Serialize(object obj)
        {
            var output = JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return _encoding.GetBytes(output);
        }
    }
}