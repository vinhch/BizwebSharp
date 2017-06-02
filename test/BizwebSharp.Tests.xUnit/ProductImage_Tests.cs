using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;
using BizwebSharp.Services;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "ProductImage")]
    public class ProductImage_Tests : IClassFixture<ProductImage_Tests_Fixture>
    {
        private ProductImage_Tests_Fixture Fixture { get; }

        public ProductImage_Tests(ProductImage_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count ProductImages")]
        public async Task Counts_ProductImages()
        {
            var count = await Fixture.Service.CountAsync(Fixture.ProductId);

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List ProductImages")]
        public async Task Lists_ProductImages()
        {
            var list = await Fixture.Service.ListAsync(Fixture.ProductId);

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Delete ProductImages")]
        public async Task Deletes_ProductImages()
        {
            var created = await Fixture.Create(true);
            bool threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(Fixture.ProductId, created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_ProductImages)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Get ProductImages")]
        public async Task Gets_ProductImages()
        {
            var image = await Fixture.Service.GetAsync(Fixture.ProductId, Fixture.Created.First().Id.Value);

            Assert.NotNull(image);
            Assert.True(image.Id.HasValue);
            Assert.Equal(Fixture.ProductId, image.ProductId);
        }

        [Fact(DisplayName = "Create ProductImages")]
        public async Task Creates_ProductImages()
        {
            var created = await Fixture.Create();

            Assert.NotNull(created);
            Assert.True(created.Id.HasValue);
            Assert.Equal(Fixture.ProductId, created.ProductId);
        }

        [Fact(DisplayName = "Update ProductImages")]
        public async Task Updates_ProductImages()
        {
            var created = await Fixture.Create();
            // API seems to refuse to set a position higher than 5.
            int position = created.Position >= 5 ? 4 : created.Position.Value + 1;
            long id = created.Id.Value;

            created.Position = position;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(created.ProductId.Value, id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(position, updated.Position);
        }
    }

    public class ProductImage_Tests_Fixture : AsyncLifetimeBizwebSharpTest<ProductImageService, ProductImage>
    {
        public ProductService ProductService => new ProductService(Utils.AuthState);

        public string ImageFileName => "image-filename.jpg";

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
                try
                {
                    await Service.DeleteAsync(ProductId, obj.Id.Value);
                }
                catch (BizwebSharpException ex)
                {
                    if (ex.HttpStatusCode != HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Failed to delete created Page with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public override async Task<ProductImage> Create(bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(ProductId, new ProductImage
            {
                Filename = ImageFileName,
                Base64 = "R0lGODlhAQABAIAAAAAAAAAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\n"
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
