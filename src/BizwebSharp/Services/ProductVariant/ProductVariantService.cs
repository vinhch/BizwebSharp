using System.Collections.Generic;
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
                    MakeRequest<int>($"products/{productId}/{ApiClassPathInPlural}/count.json", HttpMethod.GET, "count",
                        option);
        }

        public virtual async Task<IEnumerable<ProductVariant>> ListAsync(long productId, ListOption option = null)
        {
            return
                await
                    MakeRequest<List<ProductVariant>>($"products/{productId}/{ApiClassPathInPlural}.json",
                        HttpMethod.GET, ApiClassPathInPlural, option);
        }

        public virtual async Task<ProductVariant> CreateAsync(long productId, ProductVariant variant)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, variant}
            };

            return
                await
                    MakeRequest<ProductVariant>($"products/{productId}/{ApiClassPathInPlural}.json", HttpMethod.POST,
                        ApiClassPath, root);
        }

        public virtual async Task DeleteAsync(long productId, long variantId)
        {
            await MakeRequest($"products/{productId}/{ApiClassPathInPlural}/{variantId}.json", HttpMethod.DELETE);
        }
    }
}
