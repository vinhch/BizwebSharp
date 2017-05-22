using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class DiscountService : BaseServiceWithSimpleCRUD<Discount, ListOptions>
    {
        public DiscountService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
