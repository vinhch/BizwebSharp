using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Fulfillments API.
    /// </summary>
    public class FulfillmentService : BaseService
    {
        public FulfillmentService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Gets a count of all of the order's fulfillments.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="option">Options for filtering the count.</param>
        /// <returns>The count of all fulfillments for the shop.</returns>
        public virtual async Task<int> CountAsync(long orderId, ListOption option = null)
        {
            return await MakeRequestAsync<int>($"orders/{orderId}/fulfillments/count.json", HttpMethod.Get, "count", option);
        }

        /// <summary>
        /// Gets a list of up to 250 of the order's fulfillments.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="option">Options for filtering the list.</param>
        /// <returns>The list of fulfillments matching the filter.</returns>
        public virtual async Task<IEnumerable<Fulfillment>> ListAsync(long orderId, ListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<Fulfillment>>($"orders/{orderId}/fulfillments.json", HttpMethod.Get, "fulfillments",
                        option);
        }

        /// <summary>
        /// Retrieves the <see cref="Fulfillment"/> with the given id.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="fulfillmentId">The id of the Fulfillment to retrieve.</param>
        /// <param name="fields">A comma-separated list of fields to return.</param>
        /// <returns>The <see cref="Fulfillment"/>.</returns>
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

        /// <summary>
        /// Creates a new <see cref="Fulfillment"/> on the order.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="inputObject">A new <see cref="Fulfillment"/>. Id should be set to null.</param>
        /// <param name="notifyCustomer">Whether the customer should be notified that the fulfillment
        /// has been created.</param>
        /// <returns>The new <see cref="Fulfillment"/>.</returns>
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

        /// <summary>
        /// Updates the given <see cref="Fulfillment"/>.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="fulfillmentId">Id of the object being updated.</param>
        /// <param name="inputObject">The <see cref="Fulfillment"/> to update.</param>
        /// <returns>The updated <see cref="Fulfillment"/>.</returns>
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

        /// <summary>
        /// Completes a pending fulfillment with the given id.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="fulfillmentId">The fulfillment's id.</param>
        public virtual async Task<Fulfillment> CompleteAsync(long orderId, long fulfillmentId)
        {
            return
                await
                    MakeRequestAsync<Fulfillment>($"orders/{orderId}/fulfillments/{fulfillmentId}/complete..json",
                        HttpMethod.Post, "fulfillment");
        }

        /// <summary>
        /// Cancels a pending fulfillment with the given id.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="fulfillmentId">The fulfillment's id.</param>
        public virtual async Task<Fulfillment> CancelAsync(long orderId, long fulfillmentId)
        {
            return
                await
                    MakeRequestAsync<Fulfillment>($"orders/{orderId}/fulfillments/{fulfillmentId}/cancel..json",
                        HttpMethod.Post, "fulfillment");
        }
    }
}
