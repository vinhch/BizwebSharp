using System;
using System.IO;
using BizwebSharp.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace BizwebSharp.Tests.xUnit
{
    public static class Utils
    {
        private static IConfigurationRoot _configuration;
        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    var builder = new ConfigurationBuilder()
                        //.SetBasePath(env.ContentRootPath)
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.private.json", optional: true);
                        //.AddEnvironmentVariables();

                    _configuration = builder.Build();
                }
                return _configuration;
            }
        }

        private static BizwebAuthorizationState _authState;
        public static BizwebAuthorizationState AuthState
        {
            get
            {
                if (_authState == null)
                {
                    _authState = new BizwebAuthorizationState();
                    Configuration.GetSection("BizwebAuthorizationState").Bind(_authState);
                }

                if (string.IsNullOrEmpty(_authState.AccessToken) || string.IsNullOrEmpty(_authState.ApiUrl))
                {
                    throw new Exception($"{nameof(BizwebAuthorizationState)} was not initialize.");
                }

                return _authState;
            }
        }
    }
}
