using System;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "SmartCollection")]
    public class SmartCollection_Tests : IClassFixture<SmartCollection_Tests_Fixture>
    {
        private SmartCollection_Tests_Fixture Fixture { get; }

        public SmartCollection_Tests(SmartCollection_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count SmartCollections")]
        public async Task Counts_SmartCollections()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List SmartCollections")]
        public async Task Lists_SmartCollections()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Delete SmartCollections")]
        public async Task Deletes_SmartCollections()
        {
            var created = await Fixture.Create(true);
            bool threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_SmartCollections)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Get SmartCollections")]
        public async Task Gets_SmartCollections()
        {
            var created = await Fixture.Create();
            var obj = await Fixture.Service.GetAsync(created.Id.Value);

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            //Assert.Equal(Fixture.BodyHtml, obj.BodyHtml);
            Assert.Equal(Fixture.Name, obj.Name);
            Assert.StartsWith(Fixture.Alias, obj.Alias, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "Create SmartCollections")]
        public async Task Creates_SmartCollections()
        {
            var obj = await Fixture.Create();

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            //Assert.Equal(Fixture.BodyHtml, obj.BodyHtml);
            Assert.Equal(Fixture.Name, obj.Name);
            Assert.StartsWith(Fixture.Alias, obj.Alias, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "Update SmartCollections")]
        public async Task Updates_SmartCollections()
        {
            string newValue = "New Title";
            var created = await Fixture.Create();
            long id = created.Id.Value;

            created.Name = newValue;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(newValue, updated.Name);
        }
    }

    public class SmartCollection_Tests_Fixture : AsyncLifetimeBizwebSharpTest<SmartCollectionService, SmartCollection>
    {
        public string BodyHtml => "<h1>Hello world!</h1>";

        public string Alias => "BizwebSharp-Handle";

        public string Name => "BizwebSharp Test Smart Collection";

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
                        Console.WriteLine($"Failed to delete created SmartCollection with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public override async Task<SmartCollection> Create(bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(new SmartCollection
            {
                //BodyHtml = BodyHtml,
                Alias = Alias,
                Name = Name,
                Rules = new List<SmartCollectionRule>
                {
                    new SmartCollectionRule
                    {
                        Column = "variant_price",
                        Condition = "20",
                        Relation = "less_than"
                    }
                }
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
