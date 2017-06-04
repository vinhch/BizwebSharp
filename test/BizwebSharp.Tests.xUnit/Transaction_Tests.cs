using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Enums;
using BizwebSharp.Infrastructure;
using BizwebSharp.Services;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Transaction")]
    public class Transaction_Tests : IClassFixture<Transaction_Tests_Fixture>
    {
        private Transaction_Tests_Fixture Fixture { get; }

        public Transaction_Tests(Transaction_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Transactions")]
        public async Task Counts_Transactions()
        {
            var count = await Fixture.Service.CountAsync(Fixture.Created.First().OrderId.Value);

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Transactions")]
        public async Task Lists_Transactions()
        {
            var list = await Fixture.Service.ListAsync(Fixture.Created.First().OrderId.Value);

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Get Transactions")]
        public async Task Gets_Transactions()
        {
            var order = await Fixture.CreateOrder();
            var created = await Fixture.Create(order.Id.Value);
            var obj = await Fixture.Service.GetAsync(created.OrderId.Value, created.Id.Value);

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            Assert.Null(obj.ErrorCode);
            Assert.Equal(Fixture.Amount, obj.Amount);
            Assert.Equal(Fixture.Currency, obj.Currency);
            Assert.Equal(Fixture.Status, obj.Status);
        }

        [Fact(DisplayName = "Create Transactions")]
        public async Task Creates_Transactions()
        {
            var order = await Fixture.CreateOrder();
            var obj = await Fixture.Create(order.Id.Value);

            Assert.NotNull(obj);
            Assert.True(obj.Id.HasValue);
            Assert.Null(obj.ErrorCode);
            Assert.Equal(Fixture.Amount, obj.Amount);
            Assert.Equal(Fixture.Currency, obj.Currency);
            Assert.Equal(Fixture.Status, obj.Status);
        }

        [Fact(DisplayName = "Create Capture Transactions")]
        public async Task Creates_Capture_Transactions()
        {
            var kind = TransactionKind.Capture;
            var order = await Fixture.CreateOrder();
            var obj = await Fixture.Create(order.Id.Value, kind);

            Assert.Equal(TransactionStatus.Success, obj.Status);
            Assert.Equal(kind, obj.Kind);
            Assert.Null(obj.ErrorCode);
        }

        [Fact(DisplayName = "Create Refund Transactions",
            Skip = "This test returns the error 'Order cannot be refunded'. Orders that were created via API, not using a Bizweb transaction gateway, cannot be refunded. Therefore, refunds are untestable.")]
        public async Task Creates_Refund_Transactions()
        {
            var kind = TransactionKind.Refund;
            var order = await Fixture.CreateOrder();
            var obj = await Fixture.Create(order.Id.Value, kind);

            Assert.Equal(TransactionStatus.Success, obj.Status);
            Assert.Equal(kind, obj.Kind);
            Assert.Null(obj.ErrorCode);
        }

        [Fact(DisplayName = "Create A Void Transaction",
            Skip = "Transactions that aren't on store-credit or cash gateways require a parent_id.")]
        public async Task Creates_A_Void_Transaction()
        {
            var kind = TransactionKind.Void;
            var order = await Fixture.CreateOrder();
            var obj = await Fixture.Create(order.Id.Value, kind);

            Assert.Equal(TransactionStatus.Success, obj.Status);
            Assert.Equal(kind, obj.Kind);
            Assert.Null(obj.ErrorCode);
        }
    }

    public class Transaction_Tests_Fixture : IAsyncLifetime
    {
        public TransactionService Service => new TransactionService(Utils.AuthState);

        public OrderService OrderService => new OrderService(Utils.AuthState);

        public List<Transaction> Created { get; } = new List<Transaction>();

        public List<Order> CreatedOrders { get; } = new List<Order>();

        public decimal Amount => 10.00m;

        public string Currency => "VND";

        public string Gateway => "bogus";

        public TransactionStatus Status => TransactionStatus.Success;

        public long OrderId { get; set; }

        public async Task InitializeAsync()
        {
            // Create one collection for use with count, list, get, etc. tests.
            var order = await CreateOrder();
            await Create(order.Id.Value);
        }

        public async Task DisposeAsync()
        {
            foreach (var obj in CreatedOrders)
            {
                try
                {
                    await OrderService.DeleteAsync(obj.Id.Value);
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

        public async Task<Order> CreateOrder()
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
                        Price = 50
                    },
                    new LineItem()
                    {
                        Name = "Test Line Item 2",
                        Title = "Test Line Item Title 2",
                        Quantity = 2,
                        Price = 50
                    }
                },
                FinancialStatus = OrderFinancialStatus.Paid,
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = 20.00m,
                        Status = TransactionStatus.Success,
                        Kind = TransactionKind.Authorization,
                        Test = true,
                    }
                },
                Email = Guid.NewGuid().ToString() + "@example.com",
                Note = "Test note about the customer.",
            }, new OrderCreateOption
            {
                SendFulfillmentReceipt = false,
                SendReceipt = false
            });

            CreatedOrders.Add(obj);

            return obj;
        }

        /// <summary>
        /// Convenience function for running tests. Creates an object and automatically adds it to the queue for deleting after tests finish.
        /// </summary>
        public async Task<Transaction> Create(long orderId, TransactionKind kind = TransactionKind.Capture, bool skipAddToCreatedList = false)
        {
            var obj = await Service.CreateAsync(orderId, new Transaction()
            {
                Amount = Amount,
                Currency = Currency,
                Gateway = Gateway,
                Status = Status,
                Test = true,
                Kind = kind
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
