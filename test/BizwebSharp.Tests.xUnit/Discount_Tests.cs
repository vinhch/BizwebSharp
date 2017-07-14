using System.Threading.Tasks;
using BizwebSharp.Enums;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Discount")]
    public class Discount_Tests
    {
        private DiscountService _Service => new DiscountService(Utils.AuthState);

        [Fact(DisplayName = "Create An Application Credit",
            Skip = "Discount API requires a Shopify Plus account, which is difficult to use when testing a lib.")]
        public async Task Creates_An_Application_Credit()
        {
            var created = await _Service.CreateAsync(new Discount
            {
                DiscountType = DiscountType.FixedAmount,
                Value = "10.00",
                Code = "AuntieDot",
                MinimumOrderAmount = "40.00",
            });

            Assert.NotNull(created);
        }
    }
}
