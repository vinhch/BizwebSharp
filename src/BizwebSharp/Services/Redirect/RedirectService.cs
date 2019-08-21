using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Redirects API.
    /// </summary>
    public class RedirectService : BaseServiceWithSimpleCRUD<Redirect, RedirectOption>
    {
        public RedirectService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
