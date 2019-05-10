using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// Base class for Bizweb API services.
    /// Bizweb service for API that has timeline and publishable state should inherit from this class.
    /// </summary>
    /// <typeparam name="T">API entity class.</typeparam>
    /// <typeparam name="TOption">Optional API setting class.</typeparam>
    public abstract class BaseServiceHavePublishedOption<T, TOption> : BaseServiceWithSimpleCRUD<T, TOption>
        where T : BaseEntityWithTimeline, new()
        where TOption : CountOption, new()
    {
        /// <summary>
        /// Creates a new instance of BizwebService.
        /// </summary>
        /// <param name="authState">The object contain Bizweb authorization data.</param>
        protected BaseServiceHavePublishedOption(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Creates a new instance of BizwebService.
        /// </summary>
        /// <param name="authState">The object contain Bizweb authorization data.</param>
        /// <param name="apiClassPath">The API name or path, e.g. "product".</param>
        /// <param name="apiClassPathInPlural">The API name or path in plural , e.g. "products".</param>
        protected BaseServiceHavePublishedOption(BizwebAuthorizationState authState, string apiClassPath,
            string apiClassPathInPlural) : base(authState, apiClassPath, apiClassPathInPlural)
        {
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <param name="inputObject">The item being created. Id should be null.</param>
        /// <param name="option">Optional PublishedOn API setting.</param>
        public virtual async Task<T> CreateAsync(T inputObject, PublishedOnCreateOption option = null)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, inputObject}
            };

            if (option != null)
            {
                var body = inputObject.ToDictionary();
                foreach (var kvp in option.ToDictionary())
                {
                    body[kvp.Key] = kvp.Value;
                }
                root[ApiClassPath] = body;
            }

            return await MakeRequestAsync<T>($"{ApiClassPathInPlural}.json", HttpMethod.Post, ApiClassPath, root);
        }

        /// <summary>
        /// Publishes or unpublishes an item.
        /// </summary>
        /// <param name="id">The item's id.</param>
        /// <param name="isPublish">Publishes or unpublishes option.</param>
        public virtual async Task<T> PublishAsync(long id, bool isPublish = true)
        {
            var body = new
            {
                id,
                published = isPublish
            };

            var root = new Dictionary<string, object>
            {
                {ApiClassPath, body}
            };

            return await MakeRequestAsync<T>($"{ApiClassPathInPlural}/{id}.json", HttpMethod.Put, ApiClassPath, root);
        }
    }
}
