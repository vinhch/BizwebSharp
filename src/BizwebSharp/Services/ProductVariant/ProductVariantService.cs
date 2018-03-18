using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class ProductVariantService : BaseServiceWithSimpleCRUD<ProductVariant, ListOption>
    {
        public ProductVariantService(BizwebAuthorizationState authState) : base(authState, "variant", "variants")
        {
        }

        public virtual async Task<int> CountAsync(long productId, ListOption option = null)
        {
            return
                await
                    MakeRequestAsync<int>($"products/{productId}/{ApiClassPathInPlural}/count.json", HttpMethod.Get, "count",
                        option);
        }

        public virtual async Task<IEnumerable<ProductVariant>> ListAsync(long productId, ListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<ProductVariant>>($"products/{productId}/{ApiClassPathInPlural}.json",
                        HttpMethod.Get, ApiClassPathInPlural, option);
        }

        public virtual async Task<ProductVariant> CreateAsync(long productId, ProductVariant variant)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, variant}
            };

            return
                await
                    MakeRequestAsync<ProductVariant>($"products/{productId}/{ApiClassPathInPlural}.json", HttpMethod.Post,
                        ApiClassPath, root);
        }

        public virtual async Task DeleteAsync(long productId, long variantId)
        {
            await MakeRequestAsync($"products/{productId}/{ApiClassPathInPlural}/{variantId}.json", HttpMethod.Delete);
        }
    }
}
