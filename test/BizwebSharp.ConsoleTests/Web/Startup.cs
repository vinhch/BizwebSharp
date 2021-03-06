﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using BizwebSharp.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

namespace BizwebSharp.ConsoleTests.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            //app.UseReturnExceptionToJson();

            app.Use(async (context, next) =>
            {
                var sb = new StringBuilder();
                sb.Append("Get Bizweb AccessToken\n\n");
                foreach (var item in context.Request.Query)
                {
                    sb.Append($"{item.Key} = {item.Value}\n");
                }

                var bizwebValidation = MapQueryToBizwebValidation(context.Request.Query);
                var json = JsonConvert.SerializeObject(bizwebValidation, Formatting.Indented);
                sb.Append($"\nBizwebValidationModel = {json}\n");

                var requestBody = ReadRequestBody(context.Request);
                if (!string.IsNullOrWhiteSpace(requestBody))
                {
                    //prettify JSON
                    requestBody = JObject.Parse(requestBody).ToString();
                }
                sb.Append($"\nRequestBody = {requestBody}\n");

                var token = await AuthorizeAnAccessTokenAsync(bizwebValidation);
                sb.Append($"\nAccessToken = {JsonConvert.SerializeObject(token, Formatting.Indented)}\n");

                //context.Response.StatusCode = 200;
                //context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(sb.ToString(), Encoding.UTF8);

                //await next(); // if this middleware is the last, show don't need next()
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

        private static string ReadRequestBody(HttpRequest req)
        {
            // Allows using several time the stream in ASP.Net Core
            req.EnableRewind();
            req.Body.Position = 0;

            // Arguments: Stream, Encoding, detect encoding, buffer size
            // AND, the most important: keep stream opened
            string requestBody;
            using (var reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
            {
                requestBody = reader.ReadToEnd();
            }

            // Rewind, so the core is not lost when it looks the body for the request
            req.Body.Position = 0;

            return requestBody;
        }

        private static async Task<object> AuthorizeAnAccessTokenAsync(BizwebValidationModel model)
        {
            if (string.IsNullOrEmpty(model.Store))
            {
                return null;
            }

            var bwSettings = new BizwebSettings();
            Application.Configuration.GetSection("BizwebSettings").Bind(bwSettings);
            var accessToken =
                await
                    AuthorizationService.AuthorizeWithResultAsync(model.Code, model.Store, bwSettings.ApiKey,
                        bwSettings.ApiSecretKey);
            return accessToken;
        }
    }
}
