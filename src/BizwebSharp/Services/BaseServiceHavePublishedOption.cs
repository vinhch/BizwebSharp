using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Extensions;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public abstract class BaseServiceHavePublishedOption<T, TOption> : BaseServiceWithSimpleCRUD<T, TOption>
        where T : BaseEntityWithTimeline, new()
        where TOption : CountOptions, new()
    {
        protected BaseServiceHavePublishedOption(BizwebAuthorizationState authState) : base(authState)
        {
        }

        protected BaseServiceHavePublishedOption(BizwebAuthorizationState authState, string apiClassPath,
            string apiClassPathInPlural) : base(authState, apiClassPath, apiClassPathInPlural)
        {
        }

        public virtual async Task<T> CreateAsync(T inputObject, PublishedOnCreateOption option = null)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, inputObject}
            };

            if (option != null)
            {
                var body = inputObject.ToDictionary();
                foreach (var item in option.ToDictionary())
                {
                    body.Add(item);
                }
                root[ApiClassPath] = body;
            }

            return await MakeRequest<T>($"{ApiClassPathInPlural}.json", HttpMethod.POST, ApiClassPath, root);
        }

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

            return await MakeRequest<T>($"{ApiClassPathInPlural}/{id}.json", HttpMethod.PUT, ApiClassPath, root);
        }
    }
}
