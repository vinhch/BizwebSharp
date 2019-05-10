using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Customers API.
    /// </summary>
    public class CustomerService : BaseServiceWithSimpleCRUD<Customer, ListOption>
    {
        public CustomerService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Searches through a shop's customers for the given search query. NOTE: Assumes the <paramref name="query"/> and <paramref name="order"/> strings are not encoded.
        /// </summary>
        /// <param name="query">The (unencoded) search query, in format of 'Bob country:United States', which would search for customers in the United States with a name like 'Bob'.</param>
        /// <param name="order">An (unencoded) optional string to order the results, in format of 'field_name DESC'. Default is 'last_order_date DESC'.</param>
        /// <param name="option">Options for filtering the results.</param>
        /// <returns>A list of matching customers.</returns>
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

        /// <summary>
        /// Creates a new <see cref="Customer"/> on the store.
        /// </summary>
        /// <param name="inputObject">A new <see cref="Customer"/>. Id should be set to null.</param>
        /// <param name="option">Options for creating the customer.</param>
        /// <returns>The new <see cref="Customer"/>.</returns>
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

        /// <summary>
        /// Updates the given <see cref="Customer"/>.
        /// </summary>
        /// <param name="id">Id of the object being updated.</param>
        /// <param name="inputObject">The <see cref="Customer"/> to update.</param>
        /// <param name="option">Options for updating the customer.</param>
        /// <returns>The updated <see cref="Customer"/>.</returns>
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
