using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Price Rules API.
    /// </summary>
    public class PriceRuleService : BaseServiceWithSimpleCRUD<PriceRule, PriceRuleOption>
    {
        public PriceRuleService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
