using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;
using BizwebSharp.Services;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Collect")]
    public class Collect_Tests : IClassFixture<Collect_Tests_Fixture>
    {
        private Collect_Tests_Fixture Fixture { get; }

        public Collect_Tests(Collect_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Collects")]
        public async Task Counts_Collects()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.NotNull(count);
        }

        [Fact(DisplayName = "List Collects")]
        public async Task Lists_Collects()
        {
            var collects = await Fixture.Service.ListAsync();

            Assert.True(collects.Count() > 0);
        }

        [Fact(DisplayName = "List Collects With A Filter")]
        public async Task Lists_Collects_With_A_Filter()
        {
            var productId = Fixture.Created.First().ProductId;
            var collects = await Fixture.Service.ListAsync(new CollectOption
            {
                ProductId = productId,
            });

            Assert.True(collects.Count() > 0);
            Assert.All(collects, collect => Assert.True(collect.ProductId > 0));
        }

        [Fact(DisplayName = "Get Collects")]
        public async Task Gets_Collects()
        {
            var collect = await Fixture.Service.GetAsync(Fixture.Created.First().Id.Value);

            Assert.NotNull(collect);
            Assert.True(collect.Id.HasValue);
            Assert.Equal(Fixture.CollectionId, collect.CollectionId);
            Assert.True(collect.ProductId > 0);
        }

        [Fact(DisplayName = "Delete Collects")]
        public async Task Deletes_Collects()
        {
            var created = await Fixture.Create(true);
            bool thrown = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);

                var productService = new ProductService(Utils.AuthState);
                await productService.DeleteAsync(created.ProductId.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.Write($"{nameof(Deletes_Collects)} failed. {ex.Message}.");

                thrown = true;
            }

            Assert.False(thrown);
        }

        [Fact(DisplayName = "Create Collects")]
        public async Task Creates_Collects()
        {
            var collect = await Fixture.Create();

            Assert.NotNull(collect);
            Assert.True(collect.Id.HasValue);
            Assert.Equal(Fixture.CollectionId, collect.CollectionId);
            Assert.True(collect.ProductId > 0);
        }
    }

    public class Collect_Tests_Fixture : AsyncLifetimeBizwebSharpTest<CollectService, Collect>
    {
        public long CollectionId { get; set; }

        public override async Task InitializeAsync()
        {
            // Create a collection to use with these tests.
            var collection = await new CustomCollectionService(Utils.AuthState).CreateAsync(new CustomCollection()
            {
                Name = "Things",
                Published = false,
                Image = new CustomCollectionImage()
                {
                    Src = "http://placehold.it/250x250"
                }
            });
            CollectionId = collection.Id.Value;

            await base.InitializeAsync();
        }

        public override async Task DisposeAsync()
        {
            var productService = new ProductService(Utils.AuthState);

            foreach (var obj in Created)
            {
                try
                {
                    await Service.DeleteAsync(obj.Id.Value);
                    await productService.DeleteAsync(obj.ProductId.Value);
                }
                catch (BizwebSharpException ex)
                {
                    if (ex.HttpStatusCode != HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Failed to delete created Collect with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }

            // Delete the collection
            await new CustomCollectionService(Utils.AuthState).DeleteAsync(CollectionId);
        }

        public override async Task<Collect> Create(bool skipAddToCreatedList = false)
        {
            // Create a product to use with these tests.
            var product = await new ProductService(Utils.AuthState).CreateAsync(new Product
            {
                CreatedOn = DateTime.UtcNow,
                Name = "Burton Custom Freestlye 151",
                Vendor = "Burton",
                Content = "<strong>Good snowboard!</strong>",
                ProductType = "Snowboard",
                Alias = Guid.NewGuid().ToString(),
                Images = new List<ProductImage> { new ProductImage { Base64 = "R0lGODlhAQABAIAAAAAAAAAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==" } },
                PublishedScope = "published"
            });
            var obj = await Service.CreateAsync(new Collect()
            {
                CollectionId = CollectionId,
                ProductId = product.Id.Value,
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
