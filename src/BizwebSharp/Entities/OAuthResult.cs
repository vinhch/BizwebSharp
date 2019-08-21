using System.Collections.Generic;

namespace BizwebSharp
{
    /// <summary>
    /// The class representing Bizweb API OAuth result.
    /// </summary>
    public class OAuthResult
    {
        /// <summary>
        /// The Bizweb API access token
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// The granted scopes of this access token
        /// </summary>
        public IEnumerable<string> GrantedScopes { get; }

        internal OAuthResult(string accessToken, IEnumerable<string> scopes)
        {
            AccessToken = accessToken;
            GrantedScopes = scopes;
        }
    }
}
