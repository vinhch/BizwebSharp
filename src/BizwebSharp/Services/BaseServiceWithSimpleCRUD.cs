using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Extensions;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public abstract class BaseServiceWithSimpleCRUD<T, TOption> : BaseService
        where T : BaseEntityWithTimeline, new()
        where TOption : CountOptions, new()
    {
        protected string ApiClassPath { get; }

        protected string ApiClassPathInPlural { get; }

        protected BaseServiceWithSimpleCRUD(BizwebAuthorizationState authState) : base(authState)
        {
            ApiClassPath = nameof(T).ToSnakeCase();
            ApiClassPathInPlural = ApiClassPath + "s";
        }

        protected BaseServiceWithSimpleCRUD(BizwebAuthorizationState authState, string apiClassPath,
            string apiClassPathInPlural) : base(authState)
        {
            ApiClassPath = apiClassPath;
            ApiClassPathInPlural = apiClassPathInPlural;
        }

        public virtual async Task<int> CountAsync(TOption option = null)
        {
            return await MakeRequest<int>($"{ApiClassPathInPlural}/count.json", HttpMethod.GET, "count", option);
        }

        public virtual async Task<IEnumerable<T>> ListAsync(TOption option = null)
        {
            return
                await
                    MakeRequest<List<T>>($"{ApiClassPathInPlural}.json", HttpMethod.GET, ApiClassPathInPlural, option);
        }

        public virtual async Task<T> GetAsync(long id, string fields = null)
        {
            dynamic options = null;
            if (!string.IsNullOrEmpty(fields))
            {
                options = new { fields };
            }

            return await MakeRequest<T>($"{ApiClassPathInPlural}/{id}.json", HttpMethod.GET, ApiClassPath, options);
        }

        public virtual async Task<T> CreateAsync(T inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, inputObject}
            };

            return await MakeRequest<T>($"{ApiClassPathInPlural}.json", HttpMethod.POST, ApiClassPath, root);
        }

        public virtual async Task<T> UpdateAsync(long id, T inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, inputObject}
            };
            return await MakeRequest<T>($"{ApiClassPathInPlural}/{id}.json", HttpMethod.PUT, ApiClassPath, root);
        }

        public virtual async Task DeleteAsync(long id)
        {
            await MakeRequest($"{ApiClassPathInPlural}/{id}.json", HttpMethod.DELETE);
        }
    }
}
