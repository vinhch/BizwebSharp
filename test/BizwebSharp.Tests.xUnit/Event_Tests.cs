﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Options;
using BizwebSharp.Services;
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
            // Get an order id
            var orderId = (await new OrderService(Utils.AuthState).ListAsync(new OrderOption
            {
                Limit = 1
            })).First().Id.Value;

            const string subject = "order";
            const string subjectType = "orders";
            var list = await Fixture.Service.ListAsync(subjectType, orderId);

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

    public class Event_Tests_Fixture
    {
        public EventService Service => new EventService(Utils.AuthState);

        public List<Event> Created { get; } = new List<Event>();
    }
}
