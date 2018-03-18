using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
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
                    MakeRequestAsync<List<Customer>>($"{ApiClassPathInPlural}/search.json", HttpMethod.Get, ApiClassPathInPlural, option);
        }

        public virtual async Task<Customer> CreateAsync(Customer inputObject, CustomerCreateOption option)
        {
            if (option == null)
            {
                return await CreateAsync(inputObject);
            }

            var body = inputObject.ToDictionary();

            foreach (var keyValuePair in option.ToDictionary())
            {
                body[keyValuePair.Key] = keyValuePair.Value;
            }

            var root = new Dictionary<string, object>
            {
                {ApiClassPath, body}
            };

            return await MakeRequestAsync<Customer>($"{ApiClassPathInPlural}.json", HttpMethod.Post, ApiClassPath, root);
        }

        public virtual async Task<Customer> UpdateAsync(long id, Customer inputObject, CustomerUpdateOption option)
        {
            if (option == null)
            {
                return await UpdateAsync(id, inputObject);
            }

            var body = inputObject.ToDictionary();

            foreach (var keyValuePair in option.ToDictionary())
            {
                body[keyValuePair.Key] = keyValuePair.Value;
            }

            var root = new Dictionary<string, object>
            {
                {ApiClassPath, body}
            };
            return await MakeRequestAsync<Customer>($"{ApiClassPathInPlural}/{id}.json", HttpMethod.Put, ApiClassPath, root);
        }
    }
}
