using System;
using System.Linq;

namespace BizwebSharp.Infrastructure
{
    /// <summary>
    /// The class that contain Bizweb authorization data that need to connect to Bizweb API.
    /// </summary>
    public class BizwebAuthorizationState
    {
        private string _apiUrl;

        /// <summary>
        /// The base store alias
        /// </summary>
        public string Alias
        {
            get { return GetBizwebAliasFromApiUrl(ApiUrl); }
            set { ApiUrl = value == null ? null : value + ApiConst.BIZWEB_API_DOMAIN; }
        }

        /// <summary>
        /// The base store's *.bizwebvietnam.net URL.
        /// </summary>
        public string ApiUrl
        {
            get { return _apiUrl; }
            set
            {
                if (!value.Contains(ApiConst.BIZWEB_API_DOMAIN))
                    throw new FormatException($"{nameof(ApiUrl)} must have {ApiConst.BIZWEB_API_DOMAIN} domain path.");
                _apiUrl = GetBizwebAliasFromApiUrl(value) + ApiConst.BIZWEB_API_DOMAIN;
            }
        }

        /// <summary>
        /// The bizweb store access token. The token will be set as a default X-Bizweb-Access-Token for every request.
        /// </summary>
        public string AccessToken { get; set; }

        private static string GetBizwebAliasFromApiUrl(string apiUrl)
        {
            return apiUrl?.Replace("http://", string.Empty)
                .Replace("https://", string.Empty)
                .Replace("/", string.Empty)
                .Split('.')
                .First();
        }
    }
}