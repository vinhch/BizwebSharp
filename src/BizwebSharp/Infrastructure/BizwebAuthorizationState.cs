using System;
using System.Linq;

namespace BizwebSharp.Infrastructure
{
    public class BizwebAuthorizationState
    {
        private string _apiUrl;

        public string Alias
        {
            get { return GetBizwebAliasFromApiUrl(ApiUrl); }
            set { ApiUrl = value == null ? null : value + ApiConst.BIZWEB_API_DOMAIN; }
        }

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

        public string AccessToken { get; set; }

        public bool IsValidation { get; set; }

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