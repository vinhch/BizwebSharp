using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class EventService : BaseService
    {
        public EventService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(EventListOption option = null)
        {
            return await MakeRequestAsync<int>("events/count.json", HttpMethod.Get, "count", option);
        }

        public virtual async Task<IEnumerable<Event>> ListAsync(EventListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<Event>>("events.json", HttpMethod.Get, "events", option);
        }

        public virtual async Task<Event> GetAsync(long id, string fields = null)
        {
            dynamic options = null;
            if (!string.IsNullOrEmpty(fields))
            {
                options = new { fields };
            }

            return await MakeRequestAsync<Event>($"events/{id}.json", HttpMethod.Get, "event", options);
        }

        public virtual async Task<IEnumerable<Event>> ListAsync(string subjectType, long subjectId,
            EventListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<Event>>($"{subjectType}/{subjectId}/events.json", HttpMethod.Get, "events", option);
        }
    }
}
