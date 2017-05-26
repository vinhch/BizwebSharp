using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class EventService : BaseService
    {
        public EventService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(EventListOption option = null)
        {
            return await MakeRequest<int>("events/count.json", HttpMethod.GET, "count", option);
        }

        public virtual async Task<IEnumerable<Event>> ListAsync(EventListOption option = null)
        {
            return
                await
                    MakeRequest<List<Event>>("events.json", HttpMethod.GET, "event", option);
        }

        public virtual async Task<Event> GetAsync(long id, string fields = null)
        {
            dynamic options = null;
            if (!string.IsNullOrEmpty(fields))
            {
                options = new { fields };
            }

            return await MakeRequest<Event>($"events/{id}.json", HttpMethod.GET, "event", options);
        }

        public virtual async Task<IEnumerable<Event>> ListAsync(string subjectType, long subjectId,
            EventListOption option = null)
        {
            return
                await
                    MakeRequest<List<Event>>($"{subjectType}/{subjectId}/events.json", HttpMethod.GET, "event", option);
        }
    }
}
