using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class PriceRuleService : BaseServiceWithSimpleCRUD<PriceRule, PriceRuleOption>
    {
        public PriceRuleService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
