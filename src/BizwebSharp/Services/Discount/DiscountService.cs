using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Discounts API.
    /// </summary>
    public class DiscountService : BaseServiceWithSimpleCRUD<Discount, ListOption>
    {
        public DiscountService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
