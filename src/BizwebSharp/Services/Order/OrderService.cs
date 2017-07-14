using System.Collections.Generic;
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

            return await MakeRequest<int>($"{ApiClassPathInPlural}/count.json", HttpMethod.GET, "count", option);
        }

        public virtual async Task<IEnumerable<Order>> ListForCustomerAsync(long customerId, OrderOption option = null)
        {
            var optionDictionary = option.ToDictionary();
            optionDictionary["customer_id"] = customerId;

            return await MakeRequest<List<Order>>($"{ApiClassPathInPlural}.json", HttpMethod.GET, ApiClassPathInPlural, optionDictionary);
        }

        public virtual async Task<Order> CloseAsync(long id)
        {
            return await MakeRequest<Order>($"{ApiClassPathInPlural}/{id}/close.json", HttpMethod.POST, ApiClassPath);
        }

        public virtual async Task<Order> OpenAsync(long id)
        {
            return await MakeRequest<Order>($"{ApiClassPathInPlural}/{id}/open.json", HttpMethod.POST, ApiClassPath);
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

            return await MakeRequest<Order>($"{ApiClassPathInPlural}.json", HttpMethod.POST, ApiClassPath, root);
        }

        public virtual async Task CancelAsync(long id, OrderCancelOption option = null)
        {
            var root = option ?? new OrderCancelOption();
            await MakeRequest($"{ApiClassPathInPlural}/{id}/cancel.json", HttpMethod.POST, ApiClassPath, root);
        }
    }
}
