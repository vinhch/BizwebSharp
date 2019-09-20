using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Enums;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Order")]
    public class Order_Tests : IClassFixture<Order_Tests_Fixture>
    {
        private Order_Tests_Fixture Fixture { get; }

        public Order_Tests(Order_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Orders")]
        public async Task Counts_Orders()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Orders")]
        public async Task Lists_Orders()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "List Orders With Filter")]
        public async Task Lists_Orders_With_Filter()
        {
            var created = await Task.WhenAll(Enumerable.Range(0, 2).Select(i => Fixture.Create()));
            var ids = created.Select(o => o.Id.Value);
            var list = await Fixture.Service.ListAsync(new OrderOption
            {
                Ids = ids
            });

            Assert.All(list, o => Assert.Contains(o.Id.Value, ids));
        }

        [Fact(DisplayName = "Delete Orders")]
        public async Task Deletes_Orders()
        {
            var created = await Fixture.Create(true);
            bool threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_Orders)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Get Orders")]
        public async Task Gets_Orders()
        {
            var order = await Fixture.Service.GetAsync(Fixture.Created.First().Id.Value);

            Assert.NotNull(order);
            Assert.Equal(Fixture.Note, order.Note);
            Assert.True(order.Id.HasValue);
        }

        [Fact(DisplayName = "Create Orders")]
        public async Task Creates_Orders()
        {
            var created = await Fixture.Create();

            Assert.NotNull(created);
            Assert.Equal(Fixture.Note, created.Note);
            Assert.True(created.Id.HasValue);
        }

        [Fact(DisplayName = "Update Orders")]
        public async Task Updates_Orders()
        {
            string note = "This note was updated while testing BizwebSharp!";
            var created = await Fixture.Create();
            long id = created.Id.Value;

            created.Note = note;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(note, updated.Note);
        }

        [Fact(DisplayName = "Open Orders")]
        public async Task Opens_Orders()
        {
            // Close an order before opening it.
            var closed = await Fixture.Service.CloseAsync(Fixture.Created.First().Id.Value);
            var opened = await Fixture.Service.OpenAsync(closed.Id.Value);

            Assert.False(opened.ClosedOn.HasValue);
        }

        [Fact(DisplayName = "Close Orders")]
        public async Task Closes_Orders()
        {
            var closed = await Fixture.Service.CloseAsync(Fixture.Created.Last().Id.Value);

            Assert.True(closed.ClosedOn.HasValue);
        }

        [Fact(DisplayName = "Cancel Orders",
            Skip = "Implement following this document https://support.sapo.vn/phuong-thuc-post-cua-order-phan-1#cancel but always has 500 error 'invalid data or exception'")]
        public async Task Cancels_Orders()
        {
            var order = await Fixture.Create();
            bool threw = false;

            try
            {
                await Fixture.Service.CancelAsync(order.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Cancels_Orders)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Cancel Orders With Options",
            Skip = "Implement following this document https://support.sapo.vn/phuong-thuc-post-cua-order-phan-1#cancel but always has 500 error 'invalid data or exception'")]
        public async Task Cancels_Orders_With_Options()
        {
            var order = await Fixture.Create();
            bool threw = false;

            try
            {
                await Fixture.Service.CancelAsync(order.Id.Value, new OrderCancelOption
                {
                    Reason = "customer"
                });
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Cancels_Orders_With_Options)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }
    }

    public class Order_Tests_Fixture : AsyncLifetimeBizwebSharpTest<OrderService, Order>
    {
        public string Note => "This order was created while testing BizwebSharp!";

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
                        Console.WriteLine($"Failed to delete created Order with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public override async Task<Order> Create(bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(new Order()
            {
                CreatedOn = DateTime.UtcNow,
                BillingAddress = new Address()
                {
                    Address1 = "123 4th Street",
                    City = "Minneapolis",
                    Province = "Minnesota",
                    ProvinceCode = "MN",
                    Zip = "55401",
                    Phone = "555-555-5555",
                    FirstName = "John",
                    LastName = "Doe",
                    Company = "Tomorrow Corporation",
                    Country = "United States",
                    CountryCode = "US",
                    Default = true,
                },
                LineItems = new List<LineItem>()
                {
                    new LineItem()
                    {
                        Name = "Test Line Item",
                        Title = "Test Line Item Title",
                        Quantity = 2,
                        Price = 5
                    },
                    new LineItem()
                    {
                        Name = "Test Line Item 2",
                        Title = "Test Line Item Title 2",
                        Quantity = 2,
                        Price = 5
                    }
                },
                FinancialStatus = OrderFinancialStatus.Paid,
                TotalPrice = 5.00m,
                Email = Guid.NewGuid().ToString() + "@example.com",
                Note = Note,
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
