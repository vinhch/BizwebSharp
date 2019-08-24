﻿using System.Net.Http;
#if (NETSTANDARD2_0)
using Microsoft.Extensions.DependencyInjection;
#endif

namespace BizwebSharp.Helper
{
    internal static class HttpUtils
    {
#if (NETSTANDARD2_0)
        private static readonly ServiceCollection _currentServiceCollection = new ServiceCollection();
        private static ServiceProvider _currentServiceProvider;
        private static IHttpClientFactory _currentHttpClientFactory;

        private static IHttpClientFactory CreateHttpClientFactory()
        {
            if (_currentServiceProvider == null)
            {
                _currentServiceCollection.AddHttpClient();
                _currentServiceCollection.AddHttpClient("no-redirect")
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

            return _currentHttpClientFactory;
        }

        internal static HttpClient CreateHttpClient()
            => CreateHttpClientFactory().CreateClient();

        internal static HttpClient CreateHttpClientNoRedirect()
            => CreateHttpClientFactory().CreateClient("no-redirect");
#else
        // HttpClient instance need to be singleton because of this https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        private static readonly HttpClient _currentHttpClient = new HttpClient();
        internal static HttpClient CreateHttpClient() => _currentHttpClient;

        private static readonly HttpClientHandler _httpClientHandlerNoRedirect = new HttpClientHandler
        {
            AllowAutoRedirect = false
        };
        private static readonly HttpClient _httpClientNoRedirect = new HttpClient(_httpClientHandlerNoRedirect);
        internal static HttpClient CreateHttpClientNoRedirect() => _httpClientNoRedirect;
#endif
    }
}