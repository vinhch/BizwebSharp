using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Pages API.
    /// </summary>
    public class PageService : BaseServiceHavePublishedOption<Page, PageOption>
    {
        public PageService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
