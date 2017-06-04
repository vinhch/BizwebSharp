using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class PageService : BaseServiceHavePublishedOption<Page, PageOption>
    {
        public PageService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
