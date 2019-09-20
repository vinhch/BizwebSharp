using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Product")]
    public class Product_Tests : IClassFixture<Product_Tests_Fixture>
    {
        private Product_Tests_Fixture Fixture { get; }

        public Product_Tests(Product_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Products")]
        public async Task Counts_Products()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Products")]
        public async Task Lists_Products()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Delete Products")]
        public async Task Deletes_Products()
        {
            var created = await Fixture.Create(true);
            var exception = await Record.ExceptionAsync(() => Fixture.Service.DeleteAsync(created.Id.Value));

            Assert.Null(exception);
        }

        [Fact(DisplayName = "Get Products")]
        public async Task Gets_Products()
        {
            var obj = await Fixture.Service.GetAsync(Fixture.Created.First().Id.Value);

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            Assert.Equal(Fixture.Title, obj.Name);
            Assert.Equal(Fixture.Content, obj.Content);
            Assert.Equal(Fixture.ProductType, obj.ProductType);
            Assert.Equal(Fixture.Vendor, obj.Vendor);
        }

        [Fact(DisplayName = "Create Products")]
        public async Task Creates_Products()
        {
            var created = await Fixture.Create();

            Assert.NotNull(created);
            Assert.True(created.Id.HasValue);
            Assert.Equal(Fixture.Title, created.Name);
            Assert.Equal(Fixture.Content, created.Content);
            Assert.Equal(Fixture.ProductType, created.ProductType);
            Assert.Equal(Fixture.Vendor, created.Vendor);
        }

        [Fact(DisplayName = "Create Unpublished Products")]
        public async Task Creates_Unpublished_Products()
        {
            var created = await Fixture.Create(options: new ProductCreateOption
            {
                Published = false
            });

            Assert.False(created.PublishedOn.HasValue);
        }

        [Fact(DisplayName = "Update Products")]
        public async Task Updates_Products()
        {
            const string name = "BizwebSharp Updated Test Product";
            var created = await Fixture.Create();
            var id = created.Id.Value;

            created.Name = name;
            created.Id = null;
            created.Variants = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(name, updated.Name);
        }

        [Fact(DisplayName = "Publish Products")]
        public async Task Publishes_Products()
        {
            var created = await Fixture.Create(options: new ProductCreateOption
            {
                Published = false
            });
            var published = await Fixture.Service.PublishAsync(created.Id.Value);

            Assert.True(published.PublishedOn.HasValue);
        }

        [Fact(DisplayName = "Unpublish Products")]
        public async Task Unpublishes_Products()
        {
            var created = await Fixture.Create(options: new ProductCreateOption
            {
                Published = true
            });
            var unpublished = await Fixture.Service.UnpublishAsync(created.Id.Value);

            Assert.False(unpublished.PublishedOn.HasValue);
        }
    }

    public class Product_Tests_Fixture : IAsyncLifetime
    {
        public ProductService Service => new ProductService(Utils.AuthState);

        public List<Product> Created { get; } = new List<Product>();

        public string Title => "BizwebSharp Test Product";

        public string Vendor = "Auntie Dot";

        public string Content => "<strong>This product was created while testing BizwebSharp!</strong>";

        public string ProductType => "Foobars";

        public async Task InitializeAsync()
        {
            // Create one for count, list, get, etc. orders.
            await Create();
        }

        public async Task DisposeAsync()
        {
            foreach (var obj in Created)
            {
                try
                {
                    await Service.DeleteAsync(obj.Id.Value);
                }
                catch (BizwebSharpException ex)
                {
                    if (ex.HttpStatusCode != HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Failed to delete created Product with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Convenience function for running tests. Creates an object and automatically adds it to the queue for deleting after tests finish.
        /// </summary>
        public async Task<Product> Create(bool skipAddToCreateList = false, ProductCreateOption options = null)
        {
            var obj = await Service.CreateAsync(new Product()
            {
                Name = Title,
                Vendor = Vendor,
                Content = Content,
                ProductType = ProductType,
                Alias = Guid.NewGuid().ToString(),
                Images = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Base64 = "R0lGODlhAQABAIAAAAAAAAAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw=="
                    }
                },
            }, options);

            if (!skipAddToCreateList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
