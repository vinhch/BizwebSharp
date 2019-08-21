using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating a Product's Variants API.
    /// </summary>
    public class ProductVariantService : BaseServiceWithSimpleCRUD<ProductVariant, ListOption>
    {
        public ProductVariantService(BizwebAuthorizationState authState) : base(authState, "variant", "variants")
        {
        }

        /// <summary>
        /// Gets a count of all variants belonging to the given product.
        /// </summary>
        /// <param name="productId">The product that the variants belong to.</param>
        /// <param name="option">Options for filtering the result.</param>
        public virtual async Task<int> CountAsync(long productId, ListOption option = null)
        {
            return
                await
                    MakeRequestAsync<int>($"products/{productId}/{ApiClassPathInPlural}/count.json", HttpMethod.Get, "count",
                        option);
        }

        /// <summary>
        /// Gets a list of variants belonging to the given product.
        /// </summary>
        /// <param name="productId">The product that the variants belong to.</param>
        /// <param name="option">Options for filtering the result.</param>
        public virtual async Task<IEnumerable<ProductVariant>> ListAsync(long productId, ListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<ProductVariant>>($"products/{productId}/{ApiClassPathInPlural}.json",
                        HttpMethod.Get, ApiClassPathInPlural, option);
        }

        /// <summary>
        /// Creates a new <see cref="ProductVariant"/>.
        /// </summary>
        /// <param name="productId">The product that the new variant will belong to.</param>
        /// <param name="variant">A new <see cref="ProductVariant"/>. Id should be set to null.</param>
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

        /// <summary>
        /// Deletes a product variant with the given Id.
        /// </summary>
        /// <param name="productId">The product that the variant belongs to.</param>
        /// <param name="variantId">The product variant's id.</param>
        public virtual async Task DeleteAsync(long productId, long variantId)
        {
            await MakeRequestAsync($"products/{productId}/{ApiClassPathInPlural}/{variantId}.json", HttpMethod.Delete);
        }
    }
}
