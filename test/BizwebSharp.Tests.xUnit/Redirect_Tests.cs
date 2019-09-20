using System;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using FluentAssertions;
using Xunit;
using System.Linq;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Redirect")]
    public class Redirect_Tests : IClassFixture<Redirect_Tests_Fixture>
    {
        private Redirect_Tests_Fixture Fixture { get; }

        public Redirect_Tests(Redirect_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Redirects")]
        public async Task Counts_Redirects()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Redirects")]
        public async Task Lists_Redirects()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Delete Redirects")]
        public async Task Deletes_Redirects()
        {
            var created = await Fixture.Create(true);
            var exception = await Record.ExceptionAsync(() => Fixture.Service.DeleteAsync(created.Id.Value));

            Assert.Null(exception);
        }

        [Fact(DisplayName = "Get Redirects")]
        public async Task Gets_Redirects()
        {
            var created = await Fixture.Create();
            var obj = await Fixture.Service.GetAsync(created.Id.Value);

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            Assert.Equal(Fixture.Target, obj.Target);
            obj.Path.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Create Redirects")]
        public async Task Creates_Redirects()
        {
            var created = await Fixture.Create();

            Assert.NotNull(created);
            Assert.True(created.Id.HasValue);
            Assert.Equal(Fixture.Target, created.Target);
            created.Path.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Update Redirects")]
        public async Task Updates_Redirects()
        {
            string newVal = "https://example.com/updated";
            var created = await Fixture.Create();
            long id = created.Id.Value;

            created.Target = newVal;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(newVal, updated.Target);
        }
    }

    public class Redirect_Tests_Fixture : AsyncLifetimeBizwebSharpTest<RedirectService, Redirect>
    {
        public string Target => "https://www.example.com/";

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
                        Console.WriteLine($"Failed to delete created Redirect with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public override async Task<Redirect> Create(bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(new Redirect()
            {
                Path = Guid.NewGuid().ToString(),
                Target = Target,
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
