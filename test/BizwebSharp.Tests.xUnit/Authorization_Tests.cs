using System.Collections.Generic;
using BizwebSharp.Enums;
using BizwebSharp.Services.Authorization;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    public class Authorization_Tests
    {
        [Fact(DisplayName = "Builds Authorization Urls With Enums")]
        public void Builds_Authorization_Urls_With_Enums()
        {
            var scopes = new List<AuthorizationScope>()
            {
                AuthorizationScope.ReadCustomers,
                AuthorizationScope.WriteCustomers
            };
            const string redirectUrl = "http://example.com";
            var result =
                AuthorizationService.BuildAuthorizationUrl(scopes, Utils.AuthState.ApiUrl, Utils.BwSetting.ApiKey,
                    redirectUrl).ToString();

            Assert.Contains($"/admin/oauth/authorize?", result);
            Assert.Contains($"client_id={Utils.BwSetting.ApiKey}", result);
            Assert.Contains($"scope=read_customers,write_customers", result);
            Assert.Contains($"redirect_uri={redirectUrl}", result);
        }

        [Fact(DisplayName = "Builds Authorization Urls With Strings")]
        public void Builds_Authorization_Urls_With_Strings()
        {
            string[] scopes = { "read_customers", "write_customers" };
            const string redirectUrl = "http://example.com";
            var result =
                AuthorizationService.BuildAuthorizationUrl(string.Join(",", scopes), Utils.AuthState.ApiUrl,
                    Utils.BwSetting.ApiKey, redirectUrl).ToString();

            Assert.Contains($"/admin/oauth/authorize?", result);
            Assert.Contains($"client_id={Utils.BwSetting.ApiKey}", result);
            Assert.Contains($"scope=read_customers,write_customers", result);
            Assert.Contains($"redirect_uri={redirectUrl}", result);
        }
    }
}
