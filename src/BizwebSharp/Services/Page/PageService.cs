using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class PageService : BaseServiceHavePublishedOption<Page, PageOption>
    {
        public PageService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
