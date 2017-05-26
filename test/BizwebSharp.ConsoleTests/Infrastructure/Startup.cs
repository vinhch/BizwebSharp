using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using BizwebSharp.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using Newtonsoft.Json;

namespace BizwebSharp.ConsoleTests
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var bizwebValidation = MapQueryToBizwebValidation(context.Request.Query);

                var sb = new StringBuilder();
                sb.Append("Hello world\n");
                foreach (var item in context.Request.Query)
                {
                    sb.Append($"{item.Key} = {item.Value}\n");
                }

                var json = JsonConvert.SerializeObject(bizwebValidation, Formatting.Indented);
                sb.Append($"\nBizwebValidationModel = {json}\n");

                return context.Response.WriteAsync(sb.ToString());
            });
        }

        private static BizwebValidationModel MapQueryToBizwebValidation(IQueryCollection query)
        {
            var result = new BizwebValidationModel();
            var properties = typeof(BizwebValidationModel).GetProperties();
            foreach (var property in properties)
            {
                var valueAsString = query[property.Name];
                var value = Parse(valueAsString, property.PropertyType);

                if (value == null)
                    continue;

                property.SetValue(result, value, null);
            }
            return result;
        }

        private static object Parse(string valueToConvert, Type dataType)
        {
            var obj = TypeDescriptor.GetConverter(dataType);
            var value = obj.ConvertFromString(null, CultureInfo.InvariantCulture, valueToConvert);
            return value;
        }
    }
}
