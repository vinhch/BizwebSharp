using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public abstract class BaseServiceWithSimpleCRUD<T, TOption> : BaseService
        where T : BaseEntity, new()
        where TOption : CountOption, new()
    {
        protected string ApiClassPath { get; }

        protected string ApiClassPathInPlural { get; }

        protected BaseServiceWithSimpleCRUD(BizwebAuthorizationState authState) : base(authState)
        {
            ApiClassPath = typeof(T).Name.ToSnakeCase();
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
            return await MakeRequestAsync<int>($"{ApiClassPathInPlural}/count.json", HttpMethod.Get, "count", option);
        }

        public virtual async Task<IEnumerable<T>> ListAsync(TOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<T>>($"{ApiClassPathInPlural}.json", HttpMethod.Get, ApiClassPathInPlural, option);
        }

        public virtual async Task<T> GetAsync(long id, string fields = null)
        {
            dynamic options = null;
            if (!string.IsNullOrEmpty(fields))
            {
                options = new { fields };
            }

            return await MakeRequestAsync<T>($"{ApiClassPathInPlural}/{id}.json", HttpMethod.Get, ApiClassPath, options);
        }

        public virtual async Task<T> CreateAsync(T inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, inputObject}
            };

            return await MakeRequestAsync<T>($"{ApiClassPathInPlural}.json", HttpMethod.Post, ApiClassPath, root);
        }

        public virtual async Task<T> UpdateAsync(long id, T inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, inputObject}
            };
            return await MakeRequestAsync<T>($"{ApiClassPathInPlural}/{id}.json", HttpMethod.Put, ApiClassPath, root);
        }

        public virtual async Task DeleteAsync(long id)
        {
            await MakeRequestAsync($"{ApiClassPathInPlural}/{id}.json", HttpMethod.Delete);
        }
    }
}
