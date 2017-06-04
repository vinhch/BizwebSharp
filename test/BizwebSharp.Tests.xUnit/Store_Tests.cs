using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Store")]
    public class Store_Tests : IDisposable
    {
        private StoreService Service => new StoreService(Utils.AuthState);

        public void Dispose()
        {
        }

        [Fact(DisplayName = "When getting a shop")]
        public async Task Gets_Shops()
        {
            var store = await Service.GetAsync();

            Assert.NotNull(store);
            store.Name.Should().NotBeNullOrEmpty();
            store.BizwebDomain.Should().NotBeNullOrEmpty();
        }
    }
}
