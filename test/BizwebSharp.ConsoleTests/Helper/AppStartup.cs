using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BizwebSharp.ConsoleTests
{
    public static class AppStartup
    {
        public static IConfigurationRoot Configuration { get; private set; }
        public static string HostUrl { get; private set; }

        public static void End()
        {
            while (true)
            {
                Console.ReadKey();
            };
        }

        public static void Run()
        {
            Startup();

            Console.OutputEncoding = Encoding.Unicode;
            //Console.WriteLine("Press any key to start...");
            //Console.ReadKey();

            ////////////////////
            WebServerStartup();
        }

        private static void Startup()
        {
            if (Configuration == null)
            {
                // get appsettings
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile("appsettings.private.json");

                Configuration = builder.Build();
            }
        }

        private static Task WebServerStartup()
        {
            HostUrl = Configuration.GetValue<string>("HostUrl");

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
