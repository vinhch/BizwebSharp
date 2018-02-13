using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace BizwebSharp.ConsoleTests.Web
{
    public static class ReturnExceptionToJsonMiddlewareExtensions
    {
        public static IApplicationBuilder UseReturnExceptionToJson(this IApplicationBuilder builder)
        {
            return builder.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500; // or another Status accordingly to Exception Type
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;

                        await context.Response.WriteAsync(new ErrorDto()
                        {
                            Code = 500,
                            Message = ex.Message // or your custom message
                                                 // other custom data
                        }.ToString(), Encoding.UTF8);
                    }
                });
            });
        }
    }

    public class ErrorDto
    {
        public int Code { get; set; }
        public string Message { get; set; }

        // other fields

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
