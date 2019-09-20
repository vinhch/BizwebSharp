using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Enums;
using BizwebSharp.Infrastructure;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Webhook")]
    public class Webhook_Tests : IClassFixture<Webhook_Tests_Fixture>
    {
        private Webhook_Tests_Fixture Fixture { get; }

        public Webhook_Tests(Webhook_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Webhooks",
            Skip = "Always return 0. Wrong documentation or bizweb bug? https://support.sapo.vn/sapo-webhook#count")]
        public async Task Counts_Webhooks()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Webhooks")]
        public async Task Lists_Webhooks()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Delete Webhooks")]
        public async Task Deletes_Webhooks()
        {
            var created = await Fixture.Create(true);
            var threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_Webhooks)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Get Webhooks")]
        public async Task Gets_Webhooks()
        {
            var obj = await Fixture.Service.GetAsync(Fixture.Created.First().Id.Value);

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            //Assert.Equal(Fixture.Format, obj.Format);
            Assert.StartsWith(Fixture.UrlPrefix, obj.Address);
        }

        [Fact(DisplayName = "Create Webhooks")]
        public async Task Creates_Webhooks()
        {
            var obj = await Fixture.Create();

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            //Assert.Equal(Fixture.Format, obj.Format);
            Assert.StartsWith(Fixture.UrlPrefix, obj.Address);
        }

        [Fact(DisplayName = "Update Webhooks")]
        public async Task Updates_Webhooks()
        {
            var newValue = "http://mockbin.com/request?" + Guid.NewGuid();
            var created = await Fixture.Create();
            var id = created.Id.Value;

            created.Address = newValue;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(newValue, updated.Address);
        }
    }

    public class Webhook_Tests_Fixture : AsyncLifetimeBizwebSharpTest<WebhookService, Webhook>
    {
        public string UrlPrefix => "http://mockbin.com/request";

        //public string Format => "json";

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
                        Console.WriteLine($"Failed to delete created Webhook with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public override async Task<Webhook> Create(bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(new Webhook()
            {
                Address = UrlPrefix + Guid.NewGuid(),
                Fields = new string[] { "field1", "field2" },
                Format = "json",
                Topic = WebhookTopic.OrderCreated,
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
