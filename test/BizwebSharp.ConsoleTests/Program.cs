using System;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using BizwebSharp.Enums;
using BizwebSharp.Services.Authorization;
using System.Linq;

namespace BizwebSharp.ConsoleTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppStartup.Run();

            Process.Start(Hepler.GetDefaultBrowserPath(), AppStartup.HostUrl);
            RunProgram();

            AppStartup.End();
        }

        public static void RunProgram()
        {
            var bwSettings = new BizwebSettings();
            AppStartup.Configuration.GetSection("BizwebSettings").Bind(bwSettings);

            var scopes = Enum.GetValues(typeof(AuthorizationScope)).Cast<AuthorizationScope>();
            var authorizationUrl = AuthorizationService.BuildAuthorizationUrl(scopes, bwSettings.Store,
                bwSettings.ApiKey, AppStartup.HostUrl);
            //Process.Start(Hepler.GetDefaultBrowserPath(), authorizationUrl.AbsoluteUri);

            Console.WriteLine(authorizationUrl.AbsoluteUri);
        }
    }
}
