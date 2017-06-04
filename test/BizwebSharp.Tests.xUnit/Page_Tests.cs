using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Services;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Page")]
    public class Page_Tests : IClassFixture<Page_Tests_Fixture>
    {
        private Page_Tests_Fixture Fixture { get; }

        public Page_Tests(Page_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Pages")]
        public async Task Counts_Pages()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Pages")]
        public async Task Lists_Pages()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Delete Pages")]
        public async Task Deletes_Pages()
        {
            var created = await Fixture.Create(true);
            var threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_Pages)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Get Pages")]
        public async Task Gets_Pages()
        {
            var page = await Fixture.Service.GetAsync(Fixture.Created.First().Id.Value);

            Assert.NotNull(page);
            Assert.Equal(Fixture.Title, page.Title);
            Assert.Equal(Fixture.Content, page.Content);
        }

        [Fact(DisplayName = "Create Pages")]
        public async Task Creates_Pages()
        {
            var created = await Fixture.Create();

            Assert.NotNull(created);
            Assert.Equal(Fixture.Title, created.Title);
            Assert.Equal(Fixture.Content, created.Content);
        }

        [Fact(DisplayName = "Update Pages")]
        public async Task Updates_Pages()
        {
            const string html = "<h1>This string was updated while testing BizwebSharp!</h1>";
            var created = await Fixture.Create();
            var id = created.Id.Value;

            created.Content = html;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(html, updated.Content);
        }
    }

    public class Page_Tests_Fixture : AsyncLifetimeBizwebSharpTest<PageService, Page>
    {
        public string Title => "BizwebSharp Page API Tests";

        public string Content => "<strong>This string was created by BizwebSharp!</strong>";

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
                        Console.WriteLine($"Failed to delete created Page with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public override async Task<Page> Create(bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(new Page
            {
                CreatedOn = DateTime.UtcNow,
                Title = Title,
                Content = Content,
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
