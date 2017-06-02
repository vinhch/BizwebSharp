using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BizwebSharp.ConsoleTests.Web;
using BizwebSharp.Enums;
using BizwebSharp.Services.Authorization;
using Microsoft.Extensions.Configuration;

namespace BizwebSharp.ConsoleTests
{
    public class Application
    {
        public static IConfigurationRoot Configuration { get; private set; }

        public Application()
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

        public void Run()
        {
            Console.OutputEncoding = Encoding.Unicode;
            //Console.WriteLine("Press any key to start...");
            //Console.ReadKey();

            var hostUrl = Configuration.GetValue<string>("HostUrl");
            var webServer = new WebServer(hostUrl);
            webServer.Run();

            //Process.Start(Hepler.GetDefaultBrowserPath(), WebServer.HostUrl);
            OpenBizwebAuthorizationUrl();
        }

        public void End()
        {
            while (true)
            {
                Console.ReadKey();
            };
        }

        private static void OpenBizwebAuthorizationUrl()
        {
            var bwAuthorizationUrl = CreateBizwebAuthorizationUrl();
            Process.Start(Utils.GetDefaultBrowserPath(), bwAuthorizationUrl);
            Console.WriteLine(bwAuthorizationUrl);
        }

        private static string CreateBizwebAuthorizationUrl()
        {
            var bwSettings = new BizwebSettings();
            Configuration.GetSection("BizwebSettings").Bind(bwSettings);

            var scopes = Enum.GetValues(typeof(AuthorizationScope)).Cast<AuthorizationScope>();
            var authorizationUrl = AuthorizationService.BuildAuthorizationUrl(scopes, bwSettings.Store,
                bwSettings.ApiKey, WebServer.HostUrl);

            return authorizationUrl.AbsoluteUri;
        }
    }
}
