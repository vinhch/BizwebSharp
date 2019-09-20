using System;
using System.Net.Http;
using System.Threading.Tasks;
#if (NETSTANDARD2_0)
using Microsoft.Extensions.DependencyInjection;
#endif

namespace BizwebSharp.Helper
{
    internal static class HttpUtils
    {
#if (!NETSTANDARD1_4)
        #region Beware of the .NET HttpClient https://nima-ara-blog.azurewebsites.net/beware-of-the-net-httpclient/
        private const int MAX_CONNECTION_PER_SERVER = 20;
        private static readonly TimeSpan ConnectionLifeTime = TimeSpan.FromMinutes(1);

        static HttpUtils()
        {
            ConfigureServicePointManager();
        }

        private static void ConfigureServicePointManager()
        {
            // Default is 2 minutes, see https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
            System.Net.ServicePointManager.DnsRefreshTimeout = (int)ConnectionLifeTime.TotalMilliseconds;

            // Increases the concurrent outbound connections
            System.Net.ServicePointManager.DefaultConnectionLimit = MAX_CONNECTION_PER_SERVER;

#if (NET45 || NET451 || NET452 || NET46 || NET461 || NET462)
            // With .NET Framework 4.5 -> 4.6.2, it's necessary to manually enable support for TLS 1.2 to make sure.
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11
                | System.Net.SecurityProtocolType.Tls12;
#endif
        }
        #endregion
#endif

#if (NETSTANDARD2_0)
        private const string BIZWEB_NAMED_HTTPCLIENT_TYPE = "bizweb";
        private const string NO_REDIRECT_HTTPCLIENT_TYPE = "no-redirect";

        private static readonly ServiceCollection _currentServiceCollection = new ServiceCollection();
        private static ServiceProvider _currentServiceProvider;
        private static IHttpClientFactory _currentHttpClientFactory;

        private static IHttpClientFactory CreateHttpClientFactory()
        {
            if (_currentServiceProvider == null || _currentHttpClientFactory == null)
            {
                lock (_currentServiceCollection)
                {
                    if (_currentServiceProvider == null)
                    {
                        _currentServiceCollection.AddHttpClient(BIZWEB_NAMED_HTTPCLIENT_TYPE);
                        _currentServiceCollection.AddHttpClient(NO_REDIRECT_HTTPCLIENT_TYPE)
                            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                            {
                                AllowAutoRedirect = false
                            });

                        _currentServiceProvider = _currentServiceCollection.BuildServiceProvider();
                    }

                    if (_currentHttpClientFactory == null)
                    {
                        _currentHttpClientFactory = _currentServiceProvider.GetService<IHttpClientFactory>();
                    }
                }
            }

            return _currentHttpClientFactory;
        }

        internal static HttpClient CreateHttpClient()
            => CreateHttpClientFactory().CreateClient(BIZWEB_NAMED_HTTPCLIENT_TYPE);

        internal static HttpClient CreateHttpClientNoRedirect()
            => CreateHttpClientFactory().CreateClient(NO_REDIRECT_HTTPCLIENT_TYPE);
#else
        // HttpClient instance need to be singleton
        // because of this https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        private static readonly HttpClient _currentHttpClient = new HttpClient();
        internal static HttpClient CreateHttpClient() => _currentHttpClient;

        private static readonly HttpClientHandler _clientHandlerNoRedirect = new HttpClientHandler
        {
            AllowAutoRedirect = false
        };
        private static readonly HttpClient _httpClientNoRedirect = new HttpClient(_clientHandlerNoRedirect);
        internal static HttpClient CreateHttpClientNoRedirect() => _httpClientNoRedirect;
#endif

        // https://medium.com/@szntb/getting-burnt-with-httpclient-9c1712151039
        // https://josefottosson.se/you-are-probably-still-using-httpclient-wrong-and-it-is-destabilizing-your-software/
        internal static async Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request)
        {
            return await CreateHttpClient().SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }

        internal static async Task<HttpResponseMessage> SendHttpRequestNoRedirectAsync(HttpRequestMessage request)
        {
            return await CreateHttpClientNoRedirect().SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}
