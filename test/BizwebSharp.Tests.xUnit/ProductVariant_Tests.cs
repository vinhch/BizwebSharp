using System;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using Xunit;
using System.Linq;
using System.Net;
using FluentAssertions;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "ProductVariant")]
    public class ProductVariant_Tests : IClassFixture<ProductVariant_Tests_Fixture>
    {
        private ProductVariant_Tests_Fixture Fixture { get; }

        public ProductVariant_Tests(ProductVariant_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Variants")]
        public async Task Counts_Variants()
        {
            var count = await Fixture.Service.CountAsync(Fixture.ProductId);

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Variants")]
        public async Task Lists_Variants()
        {
            var list = await Fixture.Service.ListAsync(Fixture.ProductId);

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Delete Variants")]
        public async Task Deletes_Variants()
        {
            var created = await Fixture.Create(skipAddToCreatedList: true);
            bool threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(Fixture.ProductId, created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_Variants)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Get Variants")]
        public async Task Gets_Variants()
        {
            var created = await Fixture.Service.GetAsync(Fixture.Created.First().Id.Value);

            Assert.NotNull(created);
            Assert.True(created.Id.HasValue);
            Assert.Equal(Fixture.Price, created.Price);
            created.Option1.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Create Variants")]
        public async Task Creates_Variants()
        {
            var created = await Fixture.Create();

            Assert.NotNull(created);
            Assert.True(created.Id.HasValue);
            Assert.Equal(Fixture.Price, created.Price);
            created.Option1.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Update Variants")]
        public async Task Updates_Variants()
        {
            decimal newPrice = (decimal)11.22;
            var created = await Fixture.Create();
            long id = created.Id.Value;

            created.Price = newPrice;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(newPrice, updated.Price);
        }
    }

    public class ProductVariant_Tests_Fixture : AsyncLifetimeBizwebSharpTest<ProductVariantService, ProductVariant>
    {
        public decimal Price => 123.45m;

        public long ProductId { get; set; }

        public override async Task InitializeAsync()
        {
            // Get a product id to use with these tests.
            ProductId = (await new ProductService(Utils.AuthState).ListAsync(new Options.ProductOption()
            {
                Limit = 1
            })).First().Id.Value;

            await base.InitializeAsync();
        }

        public override async Task DisposeAsync()
        {
            foreach (var obj in Created)
            {
                if (!obj.Id.HasValue)
                {
                    continue;
                }

                try
                {
                    await Service.DeleteAsync(ProductId, obj.Id.Value);
                }
                catch (BizwebSharpException ex)
                {
                    if (ex.HttpStatusCode != HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Failed to delete created ProductVariant with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public override async Task<ProductVariant> Create(bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(ProductId, new ProductVariant()
            {
                Option1 = Guid.NewGuid().ToString(),
                Price = Price,
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
