using BizwebSharp.Enums;
using BizwebSharp.Infrastructure;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Theme")]
    public class Theme_Tests : IClassFixture<Theme_Tests_Fixture>
    {
        private Theme_Tests_Fixture Fixture { get; }

        public Theme_Tests(Theme_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact]
        public async Task Counts_Themes()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact]
        public async Task Lists_Themes()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(Skip = "Often fails during CI tests because stores can only have 20 themes.")]
        public async Task Deletes_Themes()
        {
            var created = await Fixture.Create(true);
            bool threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_Themes)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(Skip = "Often fails during CI tests because stores can only have 20 themes.")]
        public async Task Gets_Themes()
        {
            var created = await Fixture.Create();
            var obj = await Fixture.Service.GetAsync(created.Id.Value);

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            Assert.StartsWith(Fixture.NamePrefix, obj.Name);
            Assert.Equal(Fixture.Role, obj.Role);
        }

        [Fact(Skip = "Often fails during CI tests because stores can only have 20 themes.")]
        public async Task Creates_Themes()
        {
            var obj = await Fixture.Create();

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            Assert.StartsWith(Fixture.NamePrefix, obj.Name);
            Assert.Equal(Fixture.Role, obj.Role);
        }

        [Fact(Skip = "Often fails during CI tests because stores can only have 20 themes.")]
        public async Task Updates_Themes()
        {
            string newValue = ("BizwebSharp Updated Theme_" + Guid.NewGuid()).Substring(0, 50);
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

    public class Theme_Tests_Fixture : AsyncLifetimeBizwebSharpTest<ThemeService, Theme>
    {
        public string NamePrefix => "BizwebSharp Test Theme_";

        public ThemeRole Role => ThemeRole.Unpublished;

        public override async Task<Theme> Create(bool skipAddToCreatedList = false)
        {
            var theme = new Theme()
            {
                Name = (NamePrefix + Guid.NewGuid()).Substring(0, 50),
                Role = Role,
            };
            var obj = await Service.CreateAsync(theme);

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }

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
                        Console.WriteLine($"Failed to delete created Theme with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }
    }
}
