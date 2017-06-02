using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace BizwebSharp.ConsoleTests.Web
{
    public class WebServer
    {
        public static string HostUrl { get; private set; }

        public WebServer(string hostUrl)
        {
            HostUrl = hostUrl;
        }

        public Task Run()
        {
            return Task.Run(() =>
            {
                var builder = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls(HostUrl); ;

                var host = builder.Build();
                host.Run();
            });
        }

    }
}
