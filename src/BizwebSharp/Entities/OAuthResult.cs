using System.Collections.Generic;

namespace BizwebSharp
{
    public class OAuthResult
    {
        public string AccessToken { get; }
        public IEnumerable<string> GrantedScopes { get; }

        internal OAuthResult(string accessToken, IEnumerable<string> scopes)
        {
            AccessToken = accessToken;
            GrantedScopes = scopes;
        }
    }
}
