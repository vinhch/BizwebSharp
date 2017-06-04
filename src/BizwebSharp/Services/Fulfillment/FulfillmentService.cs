using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class FulfillmentService : BaseService
    {
        public FulfillmentService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(long orderId, ListOption option = null)
        {
            return await MakeRequest<int>($"orders/{orderId}/fulfillments/count.json", HttpMethod.GET, "count", option);
        }

        public virtual async Task<IEnumerable<Fulfillment>> ListAsync(long orderId, ListOption option = null)
        {
            return
                await
                    MakeRequest<List<Fulfillment>>($"orders/{orderId}/fulfillments.json", HttpMethod.GET, "fulfillments",
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
                    MakeRequest<Fulfillment>($"orders/{orderId}/fulfillments/{fulfillmentId}.json", HttpMethod.GET, "fulfillment",
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

            return await MakeRequest<Fulfillment>($"orders/{orderId}/fulfillments.json", HttpMethod.POST, "fulfillment", root);
        }

        public virtual async Task<Fulfillment> UpdateAsync(long orderId, long fulfillmentId, Fulfillment inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"fulfillment", inputObject}
            };
            return
                await
                    MakeRequest<Fulfillment>($"orders/{orderId}/fulfillments/{fulfillmentId}.json", HttpMethod.PUT,
                        "fulfillment", root);
        }

        public virtual async Task<Fulfillment> CompleteAsync(long orderId, long fulfillmentId)
        {
            return
                await
                    MakeRequest<Fulfillment>($"orders/{orderId}/fulfillments/{fulfillmentId}/complete..json",
                        HttpMethod.POST, "fulfillment");
        }

        public virtual async Task<Fulfillment> CancelAsync(long orderId, long fulfillmentId)
        {
            return
                await
                    MakeRequest<Fulfillment>($"orders/{orderId}/fulfillments/{fulfillmentId}/cancel..json",
                        HttpMethod.POST, "fulfillment");
        }
    }
}
