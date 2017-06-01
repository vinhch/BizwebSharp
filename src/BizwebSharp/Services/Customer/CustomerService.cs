using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Extensions;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class CustomerService : BaseServiceWithSimpleCRUD<Customer, ListOption>
    {
        public CustomerService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<IEnumerable<Customer>> SearchAsync(string query, string order = null, ListOption option = null)
        {
            IDictionary<string, object> optionDictionary = new Dictionary<string, object>
            {
                {"query", query}
            };

            if (!string.IsNullOrEmpty(order))
            {
                optionDictionary["order"] = order;
            }

            if (option != null)
            {
                foreach (var keyValuePair in option.ToDictionary())
                {
                    optionDictionary[keyValuePair.Key] = keyValuePair.Value;
                }
            }

            return
                await
                    MakeRequest<List<Customer>>($"{ApiClassPathInPlural}/search.json", HttpMethod.GET, ApiClassPathInPlural, option);
        }

        public virtual async Task<Customer> CreateAsync(Customer inputObject, CustomerCreateOption option)
        {
            var body = inputObject.ToDictionary();

            foreach (var keyValuePair in option.ToDictionary())
            {
                body[keyValuePair.Key] = keyValuePair.Value;
            }

            var root = new Dictionary<string, object>
            {
                {ApiClassPath, body}
            };

            return await MakeRequest<Customer>($"{ApiClassPathInPlural}.json", HttpMethod.POST, ApiClassPath, root);
        }

        public virtual async Task<Customer> UpdateAsync(long id, Customer inputObject, CustomerUpdateOption option)
        {
            var body = inputObject.ToDictionary();

            foreach (var keyValuePair in option.ToDictionary())
            {
                body[keyValuePair.Key] = keyValuePair.Value;
            }

            var root = new Dictionary<string, object>
            {
                {ApiClassPath, body}
            };
            return await MakeRequest<Customer>($"{ApiClassPathInPlural}/{id}.json", HttpMethod.PUT, ApiClassPath, root);
        }
    }
}
