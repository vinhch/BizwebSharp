using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Enums;
using BizwebSharp.Infrastructure;
using BizwebSharp.Services;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "ScriptTag")]
    public class ScriptTag_Tests : IClassFixture<ScriptTag_Tests_Fixture>
    {
        private ScriptTag_Tests_Fixture Fixture { get; }

        public ScriptTag_Tests(ScriptTag_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count ScriptTags")]
        public async Task Counts_ScriptTags()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List ScriptTags")]
        public async Task Lists_ScriptTags()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Delete ScriptTags")]
        public async Task Deletes_ScriptTags()
        {
            var created = await Fixture.Create(true);
            bool threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_ScriptTags)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Get ScriptTags")]
        public async Task Gets_ScriptTags()
        {
            var obj = await Fixture.Service.GetAsync(Fixture.Created.First().Id.Value);

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            Assert.Equal(Fixture.Src, obj.Src);
            Assert.Equal(Fixture.Event, obj.Event);
            //Assert.Equal(Fixture.Scope, obj.DisplayScope);
        }

        [Fact(DisplayName = "Create ScriptTags")]
        public async Task Creates_ScriptTags()
        {
            var obj = await Fixture.Create();

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            Assert.Equal(Fixture.Src, obj.Src);
            Assert.Equal(Fixture.Event, obj.Event);
            //Assert.Equal(Fixture.Scope, obj.DisplayScope);
        }

        [Fact(DisplayName = "Update ScriptTags")]
        public async Task Updates_ScriptTags()
        {
            const string newValue = "https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js";
            var created = await Fixture.Create();
            var id = created.Id.Value;

            created.Src = newValue;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            //Assert.Equal(newValue, updated.DisplayScope);
            Assert.Equal(newValue, updated.Src);
        }
    }

    public class ScriptTag_Tests_Fixture : AsyncLifetimeBizwebSharpTest<ScriptTagService, ScriptTag>
    {
        public ScriptTagEvent Event => ScriptTagEvent.Onload;

        public string Src => "https://cdnjs.cloudflare.com/ajax/libs/jquery/1.12.4/jquery.min.js";

        //public string Scope => "online_store";

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
                        Console.WriteLine($"Failed to delete created ScriptTag with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public override async Task<ScriptTag> Create(bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(new ScriptTag
            {
                Event = Event,
                Src = Src,
                //DisplayScope = Scope,
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
