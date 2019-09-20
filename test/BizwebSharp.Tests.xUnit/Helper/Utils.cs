using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BizwebSharp.Tests.xUnit
{
    public static class Utils
    {
        static Utils()
        {
            ConfigureServicePointManager();
        }

        private static readonly object _configLocker = new object();
        private static IConfigurationRoot _configuration;
        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    lock (_configLocker)
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
                    }
                }
                return _configuration;
            }
        }

        private static readonly object _authStateLocker = new object();
        private static BizwebAuthorizationState _authState;
        public static BizwebAuthorizationState AuthState
        {
            get
            {
                if (_authState == null)
                {
                    lock (_authStateLocker)
                    {
                        if (_authState == null)
                        {
                            var authState = new BizwebAuthorizationState();
                            Configuration.GetSection("BizwebAuthorizationState").Bind(authState);
                            _authState = authState;
                        }
                    }
                }

                if (string.IsNullOrEmpty(_authState.AccessToken) || string.IsNullOrEmpty(_authState.ApiUrl))
                {
                    throw new Exception($"{nameof(BizwebAuthorizationState)} was not initialize.");
                }

                return _authState;
            }
        }

        private static readonly object _bwSettingLocker = new object();
        private static BizwebSetting _bizwebSetting;
        public static BizwebSetting BwSetting
        {
            get
            {
                if (_bizwebSetting == null)
                {
                    lock (_authStateLocker)
                    {
                        if (_bizwebSetting == null)
                        {
                            _bizwebSetting = new BizwebSetting();
                            Configuration.GetSection("BizwebSetting").Bind(_bizwebSetting);
                        }
                    }
                }

                return _bizwebSetting;
            }
        }

        private static void ConfigureServicePointManager()
        {
            // Default is 2 minutes, see https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
            System.Net.ServicePointManager.DnsRefreshTimeout = (int) TimeSpan.FromMinutes(1).TotalMilliseconds; ;

            // Increases the concurrent outbound connections
            System.Net.ServicePointManager.DefaultConnectionLimit = 20;
        }

        private static readonly HttpClient _httpClient = new HttpClient();
        public static HttpClient GetHttpClient() => _httpClient;

        public static async Task<HttpResponseMessage> UploadAsync(string toUrl, string filePath,
            string body = null, string boundary = "----MyGreatBoundary")
        {
            var client = GetHttpClient();

            using (var msg = new HttpRequestMessage(HttpMethod.Post, toUrl))
            using (var multiPartContent = new MultipartFormDataContent(boundary))
            using (var stringContent = new StringContent(string.IsNullOrWhiteSpace(body) ? string.Empty : body))
            using (var fs = File.OpenRead(filePath))
            using (var streamContent = new StreamContent(fs))
            {
                if (!string.IsNullOrWhiteSpace(body))
                {
                    //Content-Disposition: form-data; name="json"
                    stringContent.Headers.Add("Content-Disposition", "form-data; name=\"json\"");
                    multiPartContent.Add(stringContent, "json");
                }

                //Content-Disposition: form-data; name="file"; filename="C:\files\596090\596090.1.mp4";
                streamContent.Headers.Add("Content-Type", "application/octet-stream");
                multiPartContent.Add(streamContent, "file", Path.GetFileName(filePath));

                //multiPartContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                msg.Content = multiPartContent;

                return await client.SendAsync(msg).ConfigureAwait(false);
            }
        }

        public static async Task<FileIoResponse> UploadToFileIoAsync(string filePath)
        {
            var response = await UploadAsync("https://file.io/?expires=1d", filePath, boundary: "----BizwebSharpTest");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FileIoResponse>(json);
        }
    }
}
