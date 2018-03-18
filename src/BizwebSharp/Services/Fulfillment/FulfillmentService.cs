using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class FulfillmentService : BaseService
    {
        public FulfillmentService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(long orderId, ListOption option = null)
        {
            return await MakeRequestAsync<int>($"orders/{orderId}/fulfillments/count.json", HttpMethod.Get, "count", option);
        }

        public virtual async Task<IEnumerable<Fulfillment>> ListAsync(long orderId, ListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<Fulfillment>>($"orders/{orderId}/fulfillments.json", HttpMethod.Get, "fulfillments",
                        option);
        }

        public virtual async Task<Fulfillment> GetAsync(long orderId, long fulfillmentId, string fields = null)
        {
            dynamic options = null;
            if (!string.IsNullOrEmpty(fields))
            {
                options = new { fields };
            }

            return
                await
                    MakeRequestAsync<Fulfillment>($"orders/{orderId}/fulfillments/{fulfillmentId}.json", HttpMethod.Get, "fulfillment",
                        options);
        }

        public virtual async Task<Fulfillment> CreateAsync(long orderId, Fulfillment inputObject, bool notifyCustomer = false)
        {
            var body = inputObject.ToDictionary();
            body["notify_customer"] = notifyCustomer;

            var root = new Dictionary<string, object>
            {
                {"fulfillment", body}
            };

            return await MakeRequestAsync<Fulfillment>($"orders/{orderId}/fulfillments.json", HttpMethod.Post, "fulfillment", root);
        }

        public virtual async Task<Fulfillment> UpdateAsync(long orderId, long fulfillmentId, Fulfillment inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"fulfillment", inputObject}
            };
            return
                await
                    MakeRequestAsync<Fulfillment>($"orders/{orderId}/fulfillments/{fulfillmentId}.json", HttpMethod.Put,
                        "fulfillment", root);
        }

        public virtual async Task<Fulfillment> CompleteAsync(long orderId, long fulfillmentId)
        {
            return
                await
                    MakeRequestAsync<Fulfillment>($"orders/{orderId}/fulfillments/{fulfillmentId}/complete..json",
                        HttpMethod.Post, "fulfillment");
        }

        public virtual async Task<Fulfillment> CancelAsync(long orderId, long fulfillmentId)
        {
            return
                await
                    MakeRequestAsync<Fulfillment>($"orders/{orderId}/fulfillments/{fulfillmentId}/cancel..json",
                        HttpMethod.Post, "fulfillment");
        }
    }
}
