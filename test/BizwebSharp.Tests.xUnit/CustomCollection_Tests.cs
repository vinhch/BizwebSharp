using System;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Services;
using Xunit;
using System.Linq;
using System.Net;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "CustomCollection")]
    public class CustomCollection_Tests : IClassFixture<CustomCollection_Tests_Fixture>
    {
        private CustomCollection_Tests_Fixture Fixture { get; }

        public CustomCollection_Tests(CustomCollection_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count CustomCollections")]
        public async Task Counts_CustomCollections()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List CustomCollections")]
        public async Task Lists_CustomCollections()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Get CustomCollections")]
        public async Task Gets_CustomCollections()
        {
            var collection = await Fixture.Service.GetAsync(Fixture.Created.First().Id.Value);

            Assert.NotNull(collection);
            Assert.True(collection.Id.HasValue);
            Assert.Equal(Fixture.Name, collection.Name);
        }

        [Fact(DisplayName = "Delete CustomCollections")]
        public async Task Deletes_CustomCollections()
        {
            var created = await Fixture.Create(true);
            bool threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_CustomCollections)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Create CustomCollections")]
        public async Task Creates_CustomCollections()
        {
            var collection = await Fixture.Create();

            Assert.NotNull(collection);
            Assert.True(collection.Id.HasValue);
            Assert.Equal(Fixture.Name, collection.Name);
        }

        [Fact(DisplayName = "Update CustomCollections")]
        public async Task Updates_CustomCollections()
        {
            string newTitle = "New Title";
            var created = await Fixture.Create();
            long id = created.Id.Value;

            created.Name = newTitle;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(newTitle, updated.Name);
        }
    }

    public class CustomCollection_Tests_Fixture : AsyncLifetimeBizwebSharpTest<CustomCollectionService, CustomCollection>
    {
        public string Name => "Things";

        public override async Task DisposeAsync()
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
                        Console.WriteLine($"Failed to delete created CustomCollection with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public override async Task<CustomCollection> Create(bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(new CustomCollection
            {
                Name = Name,
                Published = false
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
