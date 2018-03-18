using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace BizwebSharp.Infrastructure
{
    public class JsonContent : StringContent
    {
        private readonly object _data;

        public JsonContent(string content) : base(content)
        {
        }

        public JsonContent(string content, Encoding encoding) : base(content, encoding)
        {
        }

        public JsonContent(string content, Encoding encoding, string mediaType) : base(content, encoding, mediaType)
        {
        }

        public JsonContent(object data) : this(ToJson(data), Encoding.UTF8, "application/json")
        {
            _data = data;
        }

        private static string ToJson(object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public JsonContent Clone()
        {
            return new JsonContent(_data);
        }
    }
}
