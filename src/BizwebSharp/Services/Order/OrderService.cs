using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class OrderService : BaseServiceWithSimpleCRUD<Order, OrderOption>
    {
        public OrderService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountForCustomerAsync(long customerId, OrderOption option = null)
        {
            var optionDictionary = option.ToDictionary();
            optionDictionary["customer_id"] = customerId;

            return await MakeRequestAsync<int>($"{ApiClassPathInPlural}/count.json", HttpMethod.Get, "count", option);
        }

        public virtual async Task<IEnumerable<Order>> ListForCustomerAsync(long customerId, OrderOption option = null)
        {
            var optionDictionary = option.ToDictionary();
            optionDictionary["customer_id"] = customerId;

            return await MakeRequestAsync<List<Order>>($"{ApiClassPathInPlural}.json", HttpMethod.Get, ApiClassPathInPlural, optionDictionary);
        }

        public virtual async Task<Order> CloseAsync(long id)
        {
            return await MakeRequestAsync<Order>($"{ApiClassPathInPlural}/{id}/close.json", HttpMethod.Post, ApiClassPath);
        }

        public virtual async Task<Order> OpenAsync(long id)
        {
            return await MakeRequestAsync<Order>($"{ApiClassPathInPlural}/{id}/open.json", HttpMethod.Post, ApiClassPath);
        }

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

        public virtual async Task CancelAsync(long id, OrderCancelOption option = null)
        {
            var root = option ?? new OrderCancelOption();
            await MakeRequestAsync($"{ApiClassPathInPlural}/{id}/cancel.json", HttpMethod.Post, ApiClassPath, root);
        }
    }
}
