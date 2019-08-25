using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// Base class for Bizweb API services.
    /// This base class already has simple CRUD method to API
    /// like Get, Create, Update, Delete, List, Count
    /// </summary>
    /// <typeparam name="T">API entity class.</typeparam>
    /// <typeparam name="TOption">Optional API setting class.</typeparam>
    public abstract class BaseServiceWithSimpleCRUD<T, TOption> : BaseService
        where T : BaseEntity, new()
        where TOption : CountOption, new()
    {
        /// <summary>
        /// The API name or path, e.g. "product".
        /// </summary>
        protected string ApiClassPath { get; }

        /// <summary>
        /// The API name or path in plural , e.g. "products".
        /// </summary>
        protected string ApiClassPathInPlural { get; }

        /// <summary>
        /// Creates a new instance of BizwebService.
        /// </summary>
        /// <param name="authState">The object contain Bizweb authorization data.</param>
        protected BaseServiceWithSimpleCRUD(BizwebAuthorizationState authState) : base(authState)
        {
            ApiClassPath = typeof(T).Name.ToSnakeCase();
            ApiClassPathInPlural = ApiClassPath + "s";
        }

        /// <summary>
        /// Creates a new instance of BizwebService.
        /// </summary>
        /// <param name="authState">The object contain Bizweb authorization data.</param>
        /// <param name="apiClassPath">The API name or path, e.g. "product".</param>
        /// <param name="apiClassPathInPlural">The API name or path in plural , e.g. "products".</param>
        protected BaseServiceWithSimpleCRUD(BizwebAuthorizationState authState, string apiClassPath,
            string apiClassPathInPlural) : base(authState)
        {
            ApiClassPath = apiClassPath;
            ApiClassPathInPlural = apiClassPathInPlural;
        }

        /// <summary>
        /// Gets a count of all items in API.
        /// </summary>
        /// <param name="option">Optional Count API setting.</param>
        public virtual async Task<int> CountAsync(TOption option = null)
        {
            return await MakeRequestAsync<int>($"{ApiClassPathInPlural}/count.json", HttpMethod.Get, "count", option);
        }

        /// <summary>
        /// Gets a list of up to 250 items.
        /// </summary>
        /// <param name="option">Optional List API setting.</param>
        public virtual async Task<IEnumerable<T>> ListAsync(TOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<T>>($"{ApiClassPathInPlural}.json", HttpMethod.Get, ApiClassPathInPlural, option);
        }

        /// <summary>
        /// Gets an item with the given id.
        /// </summary>
        /// <param name="id">The id of the item you want to retrieve.</param>
        /// <param name="fields">Optional comma-separated list of fields to return.</param>
        public virtual async Task<T> GetAsync(long id, string fields = null)
        {
            var options = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(fields))
            {
                options["fields"] = fields;
            }

            return await MakeRequestAsync<T>($"{ApiClassPathInPlural}/{id}.json", HttpMethod.Get, ApiClassPath, options);
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <param name="inputObject">The item being created. Id should be null.</param>
        public virtual async Task<T> CreateAsync(T inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, inputObject}
            };

            return await MakeRequestAsync<T>($"{ApiClassPathInPlural}.json", HttpMethod.Post, ApiClassPath, root);
        }

        /// <summary>
        /// Updates an item.
        /// </summary>
        /// <param name="id">The id of the item you want to update.</param>
        /// <param name="inputObject">The updated blog. Id should not be null.</param>
        public virtual async Task<T> UpdateAsync(long id, T inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, inputObject}
            };
            return await MakeRequestAsync<T>($"{ApiClassPathInPlural}/{id}.json", HttpMethod.Put, ApiClassPath, root);
        }

        /// <summary>
        /// Deletes a item with the given id.
        /// </summary>
        /// <param name="id">The id of the item you want to delete.</param>
        public virtual async Task DeleteAsync(long id)
        {
            await MakeRequestAsync($"{ApiClassPathInPlural}/{id}.json", HttpMethod.Delete);
        }
    }
}
