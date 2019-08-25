using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for getting Events API.
    /// Reference: https://help.shopify.com/api/reference/event
    /// </summary>
    public class EventService : BaseService
    {
        public EventService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Gets a count of all site events.
        /// </summary>
        /// <param name="option">Options for filtering the result.</param>
        /// <returns>The count of all site events.</returns>
        public virtual async Task<int> CountAsync(EventListOption option = null)
        {
            return await MakeRequestAsync<int>("events/count.json", HttpMethod.Get, "count", option);
        }

        /// <summary>
        /// Returns a list of events.
        /// </summary>
        /// <param name="option">Options for filtering the result.</param>
        public virtual async Task<IEnumerable<Event>> ListAsync(EventListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<Event>>("events.json", HttpMethod.Get, "events", option);
        }

        public virtual async Task<Event> GetAsync(long id, string fields = null)
        {
            var options = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(fields))
            {
                options["fields"] = fields;
            }

            return await MakeRequestAsync<Event>($"events/{id}.json", HttpMethod.Get, "event", options);
        }

        /// <summary>
        /// Returns a list of events for the given subject and subject type. A full list of supported subject types can be found at https://help.shopify.com/api/reference/event
        /// </summary>
        /// <param name="option">Options for filtering the result.</param>
        /// <param name="subjectId">Restricts results to just one subject item, e.g. all changes on a product.</param>
        /// <param name="subjectType">
        /// The subject's type, e.g. 'Order' or 'Product'.
        /// Known subject types are 'Articles', 'Blogs', 'Custom_Collections', 'Comments', 'Orders', 'Pages',
        /// 'Products' and 'Smart_Collections'.
        /// A current list of subject types can be found at https://help.shopify.com/api/reference/event
        /// </param>
        public virtual async Task<IEnumerable<Event>> ListAsync(string subjectType, long subjectId,
            EventListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<Event>>($"{subjectType}/{subjectId}/events.json", HttpMethod.Get, "events", option);
        }
    }
}
