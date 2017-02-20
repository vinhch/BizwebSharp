using System;
using System.Linq;
using BizwebSharp.Const;

namespace BizwebSharp.Infrastructure
{
    public class BizwebAuthorizationState
    {
        private string _apiUrl;

        public string Alias
        {
            get { return GetBizwebAliasFromApiUrl(ApiUrl); }
            set { ApiUrl = value == null ? null : value + ApiConst.BizwebApiDomain; }
        }

        public string ApiUrl
        {
            get { return _apiUrl; }
            set
            {
                if (!value.Contains(ApiConst.BizwebApiDomain))
                    throw new FormatException($"{nameof(ApiUrl)} must have {ApiConst.BizwebApiDomain} domain path.");
                _apiUrl = GetBizwebAliasFromApiUrl(value) + ApiConst.BizwebApiDomain;
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