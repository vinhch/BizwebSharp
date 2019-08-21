using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Orders API.
    /// </summary>
    public class OrderService : BaseServiceWithSimpleCRUD<Order, OrderOption>
    {
        public OrderService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Gets a count of all of the order's customers.
        /// </summary>
        /// <param name="customerId">The id of the customer to list orders for.</param>
        /// <param name="option">Options for filtering the list.</param>
        /// <returns>The count of all customers for the order.</returns>
        public virtual async Task<int> CountForCustomerAsync(long customerId, OrderOption option = null)
        {
            var optionDictionary = option.ToDictionary();
            optionDictionary["customer_id"] = customerId;

            return await MakeRequestAsync<int>($"{ApiClassPathInPlural}/count.json", HttpMethod.Get, "count", option);
        }

        /// <summary>
        /// Gets a list of up to 250 of the customer's orders.
        /// </summary>
        /// <param name="customerId">The id of the customer to list orders for.</param>
        /// <param name="option">Options for filtering the list.</param>
        /// <returns>The list of orders matching the filter.</returns>
        public virtual async Task<IEnumerable<Order>> ListForCustomerAsync(long customerId, OrderOption option = null)
        {
            var optionDictionary = option.ToDictionary();
            optionDictionary["customer_id"] = customerId;

            return await MakeRequestAsync<List<Order>>($"{ApiClassPathInPlural}.json", HttpMethod.Get, ApiClassPathInPlural, optionDictionary);
        }

        /// <summary>
        /// Closes an order.
        /// </summary>
        /// <param name="id">The order's id.</param>
        public virtual async Task<Order> CloseAsync(long id)
        {
            return await MakeRequestAsync<Order>($"{ApiClassPathInPlural}/{id}/close.json", HttpMethod.Post, ApiClassPath);
        }

        /// <summary>
        /// Opens a closed order.
        /// </summary>
        /// <param name="id">The order's id.</param>
        public virtual async Task<Order> OpenAsync(long id)
        {
            return await MakeRequestAsync<Order>($"{ApiClassPathInPlural}/{id}/open.json", HttpMethod.Post, ApiClassPath);
        }

        /// <summary>
        /// Creates a new <see cref="Order"/> on the store.
        /// </summary>
        /// <param name="order">A new <see cref="Order"/>. Id should be set to null.</param>
        /// <param name="option">Options for creating the order.</param>
        /// <returns>The new <see cref="Order"/>.</returns>
        public virtual async Task<Order> CreateAsync(Order order, OrderCreateOption option)
        {
            var body = order.ToDictionary();
            foreach (var kvp in option.ToDictionary())
            {
                body[kvp.Key] = kvp.Value;
            }

            var root = new Dictionary<string, object>
            {
                {ApiClassPath, body}
            };

            return await MakeRequestAsync<Order>($"{ApiClassPathInPlural}.json", HttpMethod.Post, ApiClassPath, root);
        }

        /// <summary>
        /// Cancels an order.
        /// </summary>
        /// <param name="id">The order's id.</param>
        /// <param name="option">Options for canceling the order.</param>
        /// <returns>The cancelled <see cref="Order"/>.</returns>
        public virtual async Task CancelAsync(long id, OrderCancelOption option = null)
        {
            var root = option ?? new OrderCancelOption();
            await MakeRequestAsync($"{ApiClassPathInPlural}/{id}/cancel.json", HttpMethod.Post, ApiClassPath, root);
        }
    }
}
