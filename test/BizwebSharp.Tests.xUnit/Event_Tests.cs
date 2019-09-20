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
    [Trait("Category", "Event")]
    public class Event_Tests : IClassFixture<Event_Tests_Fixture>
    {
        private Event_Tests_Fixture Fixture { get; }

        public Event_Tests(Event_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Events")]
        public async Task Counts_Events()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Events")]
        public async Task Lists_Events()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "List Events For Subjects")]
        public async Task Lists_Events_For_Subjects()
        {
            const string subject = "order";
            const string subjectType = "orders";
            var list = await Fixture.Service.ListAsync(subjectType, Fixture.OrderId.Value);

            Assert.NotNull(list);
            Assert.All(list, e => Assert.Equal(subject, e.SubjectType));
        }

        [Fact(DisplayName = "Get Events")]
        public async Task Gets_Events()
        {
            var list = await Fixture.Service.ListAsync(option: new EventListOption
            {
                Limit = 1
            });
            var evt = await Fixture.Service.GetAsync(list.First().Id.Value);

            Assert.NotNull(evt);
            Assert.NotNull(evt.Author);
            Assert.True(evt.CreatedOn.HasValue);
            Assert.NotNull(evt.Message);
            Assert.True(evt.SubjectId > 0);
            Assert.NotNull(evt.SubjectType);
            Assert.NotNull(evt.Verb);
        }
    }

    public class Event_Tests_Fixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            // Create at least one Order for Event tests.
            OrderId = (await CreateAOrder()).Id.Value;
        }

        public async Task DisposeAsync()
        {
            await CleanUpOrdersSetup();
        }

        private async Task CleanUpOrdersSetup()
        {
            foreach (var order in CreatedOrders)
            {
                try
                {
                    await OrderService.DeleteAsync(order.Id.Value);
                }
                catch (BizwebSharpException ex)
                {
                    if (ex.HttpStatusCode != HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Failed to delete Order with id {order.Id.Value}. {ex.Message}");
                    }
                }
            }

            var customerService = new CustomerService(Utils.AuthState);
            foreach (var obj in CreatedCustomers)
            {
                try
                {
                    await customerService.DeleteAsync(obj.Id.Value);
                }
                catch (BizwebSharpException ex)
                {
                    if (ex.HttpStatusCode != HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Failed to delete created Customer with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public long? OrderId { get; set; }

        public EventService Service => new EventService(Utils.AuthState);

        public OrderService OrderService => new OrderService(Utils.AuthState);

        public List<Order> CreatedOrders { get; } = new List<Order>();

        public List<Customer> CreatedCustomers { get; } = new List<Customer>();

        public async Task<Order> CreateAOrder(bool skipAddToCreatedList = false)
        {
            var obj = await OrderService.CreateAsync(new Order()
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
                Email = Guid.NewGuid() + "@example.com",
                Note = "This order was created while testing BizwebSharp!",
            });

            if (!skipAddToCreatedList)
            {
                CreatedOrders.Add(obj);
            }
            CreatedCustomers.Add(obj.Customer);

            return obj;
        }
    }
}
