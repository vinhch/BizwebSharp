using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class PriceRuleService : BaseServiceWithSimpleCRUD<PriceRule, PriceRuleOption>
    {
        protected PriceRuleService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
