using BizwebSharp.Infrastructure;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "PriceRule")]
    public class PriceRule_Tests : IClassFixture<PriceRule_Tests_Fixture>
    {
        private PriceRule_Tests_Fixture Fixture { get; }

        public PriceRule_Tests(PriceRule_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count PriceRules")]
        public async Task Counts_PriceRules()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List PriceRules")]
        public async Task Lists_PriceRules()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Get PriceRules")]
        public async Task Gets_PriceRules()
        {
            var created = Fixture.Created.First();
            var rule = await Fixture.Service.GetAsync(created.Id.Value);

            Assert.NotNull(rule);
            Assert.Equal(created.ValueType, rule.ValueType);
            Assert.Equal(created.TargetType, rule.TargetType);
            Assert.Equal(created.TargetSelection, rule.TargetSelection);
            Assert.Equal(created.AllocationMethod, rule.AllocationMethod);
            Assert.Equal(created.Value, rule.Value);
        }

        [Fact(DisplayName = "Create PriceRules")]
        public async Task Creates_PriceRules()
        {
            var created = await Fixture.Create();

            Assert.NotNull(created);
            Assert.StartsWith(Fixture.TitlePrefix, created.Title);
            Assert.Equal(Fixture.ValueType, created.ValueType);
            Assert.Equal(Fixture.TargetType, created.TargetType);
            Assert.Equal(Fixture.TargetSelection, created.TargetSelection);
            Assert.Equal(Fixture.AllocationMethod, created.AllocationMethod);
            Assert.Equal(Fixture.Value, created.Value);
        }

        [Fact(DisplayName = "Delete PriceRules")]
        public async Task Deletes_PriceRules()
        {
            var created = await Fixture.Create(true);
            bool threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_PriceRules)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Update PriceRules")]
        public async Task Updates_PriceRules()
        {
            var created = await Fixture.Create();
            long id = created.Id.Value;

            created.Value = -5.0m;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(created.Value, updated.Value);
        }
    }

    public class PriceRule_Tests_Fixture : AsyncLifetimeBizwebSharpTest<PriceRuleService, PriceRule>
    {
        public string TitlePrefix => "BizwebSharp PriceRule_";

        public string ValueType => "percentage";

        public string TargetType => "line_item";

        public string TargetSelection => "all";

        public string AllocationMethod => "across";

        public decimal Value => -10.0m;

        public string CustomerSelection => "all";

        public bool OncePerCustomer => false;

        public override async Task<PriceRule> Create(bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(new PriceRule
            {
                Title = this.TitlePrefix + Guid.NewGuid().ToString(),
                ValueType = this.ValueType,
                TargetType = this.TargetType,
                TargetSelection = this.TargetSelection,
                AllocationMethod = this.AllocationMethod,
                Value = this.Value,
                CustomerSelection = this.CustomerSelection,
                OncePerCustomer = this.OncePerCustomer,
                PrerequisiteSubtotalRange = new PrerequisiteSubtotalRange
                {
                    GreaterThanOrEqualTo = 40
                },
                StartsOn = new DateTimeOffset(DateTime.Now)
            });

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
                        Console.WriteLine($"Failed to delete created PriceRule with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }
    }
}
