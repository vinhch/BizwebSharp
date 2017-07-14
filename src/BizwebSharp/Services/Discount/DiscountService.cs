using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class DiscountService : BaseServiceWithSimpleCRUD<Discount, ListOption>
    {
        public DiscountService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
