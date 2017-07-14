using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using FluentAssertions;
using Xunit;
using System.Linq;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Customer")]
    public class Customer_Tests : IClassFixture<Customer_Tests_Fixture>
    {
        private Customer_Tests_Fixture Fixture { get; }

        public Customer_Tests(Customer_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Customers")]
        public async Task Counts_Customers()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Customers")]
        public async Task Lists_Customers()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Delete Customers")]
        public async Task Deletes_Customers()
        {
            var created = await Fixture.Create();
            var threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_Customers)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Get Customers")]
        public async Task Gets_Customers()
        {
            var customer = await Fixture.Service.GetAsync(Fixture.Created.First().Id.Value);

            Assert.NotNull(customer);
            Assert.Equal(Fixture.FirstName, customer.FirstName);
            Assert.Equal(Fixture.LastName, customer.LastName);
            Assert.Equal(Fixture.Note, customer.Note);
            Assert.NotNull(customer.Addresses);
            Assert.NotNull(customer.DefaultAddress);
        }

        [Fact(DisplayName = "Get Customers With Options")]
        public async Task Gets_Customers_With_Options()
        {
            var customer = await Fixture.Service.GetAsync(Fixture.Created.First().Id.Value, "first_name,last_name");

            Assert.NotNull(customer);
            Assert.Equal(Fixture.FirstName, customer.FirstName);
            Assert.Equal(Fixture.LastName, customer.LastName);
            customer.Note.Should().BeNullOrEmpty();
            customer.Addresses.Should().BeNullOrEmpty();
            Assert.Null(customer.DefaultAddress);

        }

        [Fact(DisplayName = "Create Customers")]
        public async Task Creates_Customers()
        {
            var customer = await Fixture.Create();

            Assert.NotNull(customer);
            Assert.Equal(Fixture.FirstName, customer.FirstName);
            Assert.Equal(Fixture.LastName, customer.LastName);
            Assert.Equal(Fixture.Note, customer.Note);
            Assert.NotNull(customer.Addresses);
        }

        [Fact(DisplayName = "Create Customers With Options")]
        public async Task Creates_Customers_With_Options()
        {
            var customer = await Fixture.Create(options: new CustomerCreateOption
            {
                Password = "loktarogar",
                PasswordConfirmation = "loktarogar",
                SendEmailInvite = false,
                SendWelcomeEmail = false,
            });

            Assert.NotNull(customer);
            Assert.Equal(Fixture.FirstName, customer.FirstName);
            Assert.Equal(Fixture.LastName, customer.LastName);
            Assert.Equal(Fixture.Note, customer.Note);
            Assert.NotNull(customer.Addresses);
        }

        [Fact(DisplayName = "Update Customers")]
        public async Task Updates_Customers()
        {
            const string firstName = "Jane";
            var created = await Fixture.Create();
            var id = created.Id.Value;

            created.FirstName = firstName;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(firstName, updated.FirstName);
        }

        [Fact(DisplayName = "Update Customers With Options")]
        public async Task Updates_Customers_With_Options()
        {
            const string firstName = "Jane";
            var created = await Fixture.Create();
            var id = created.Id.Value;

            created.FirstName = firstName;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created, new CustomerCreateOption
            {
                Password = "loktarogar",
                PasswordConfirmation = "loktarogar"
            });

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(firstName, updated.FirstName);
        }

        [Fact(DisplayName = "Search For Customers")]
        public async Task Searches_For_Customers()
        {
            // It takes anywhere between 3 seconds to 30 seconds for Shopify to index new customers for searches.
            // Rather than putting a 20 second Thread.Sleep in the test, we'll just assume it's successful if the
            // test doesn't throw an exception.
            bool threw = false;

            try
            {
                var search = await Fixture.Service.SearchAsync("John");
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Searches_For_Customers)} failed. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }
    }

    public class Customer_Tests_Fixture : IAsyncLifetime
    {
        public CustomerService Service => new CustomerService(Utils.AuthState);

        public List<Customer> Created { get; } = new List<Customer>();

        public string FirstName => "John";

        public string LastName => "Doe";

        public string Note => "Test note about this customer.";

        public async Task InitializeAsync()
        {
            // Create one customer for use with count, list, get, etc. tests.
            await Create();
        }

        public async Task DisposeAsync()
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
                        Console.WriteLine($"Failed to delete created Customer with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public async Task<Customer> Create(bool skipAddToCreatedList = false, CustomerCreateOption options = null)
        {
            var obj = await Service.CreateAsync(new Customer()
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Guid.NewGuid().ToString() + "@example.com",
                Addresses = new List<Address>()
                {
                    new Address()
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
                    }
                },
                VerifiedEmail = true,
                Note = Note,
                State = "enabled"
            }, options);

            if (!skipAddToCreatedList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
