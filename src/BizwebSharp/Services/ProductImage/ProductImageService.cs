using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating ProductImages API.
    /// </summary>
    public class ProductImageService : BaseService
    {
        public ProductImageService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Gets a count of all of the shop's ProductImages.
        /// </summary>
        /// <param name="productId">The id of the product that counted images belong to.</param>
        /// <param name="option">An optional filter that restricts the results.</param>
        /// <returns>The count of all ProductImages for the shop.</returns>
        public virtual async Task<int> CountAsync(long productId, PublishableListOption option = null)
        {
            return await MakeRequestAsync<int>($"products/{productId}/images/count.json", HttpMethod.Get, "count", option);
        }

        /// <summary>
        /// Gets a list of up to 250 of the shop's ProductImages.
        /// </summary>
        /// <param name="productId">The id of the product that counted images belong to.</param>
        /// <param name="option">An optional filter that restricts the results.</param>
        /// <returns>The list of <see cref="ProductImage"/> objects.</returns>
        public virtual async Task<IEnumerable<ProductImage>> ListAsync(long productId, PublishableListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<ProductImage>>($"products/{productId}/images.json", HttpMethod.Get, "images",
                        option);
        }

        /// <summary>
        /// Retrieves the <see cref="ProductImage"/> with the given id.
        /// </summary>
        /// <param name="productId">The id of the product that counted images belong to.</param>
        /// <param name="imageId">The id of the ProductImage to retrieve.</param>
        /// <param name="fields">A comma-separated list of fields to return.</param>
        /// <returns>The <see cref="ProductImage"/>.</returns>
        public virtual async Task<ProductImage> GetAsync(long productId, long imageId, string fields = null)
        {
            var options = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(fields))
            {
                options["fields"] = fields;
            }

            return
                await
                    MakeRequestAsync<ProductImage>($"products/{productId}/images/{imageId}.json", HttpMethod.Get, "image",
                        options);
        }

        /// <summary>
        /// Creates a new <see cref="ProductImage"/>. If the new image has an <see cref="ProductImage.Base64"/> string, it will be converted to the <see cref="ProductImage.Src"/>.
        /// </summary>
        /// <param name="productId">The id of the product that counted images belong to.</param>
        /// <param name="inputObject">The new <see cref="ProductImage"/>.</param>
        /// <returns>The new <see cref="ProductImage"/>.</returns>
        public virtual async Task<ProductImage> CreateAsync(long productId, ProductImage inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"image", inputObject}
            };

            return await MakeRequestAsync<ProductImage>($"products/{productId}/images.json", HttpMethod.Post, "image", root);
        }

        /// <summary>
        /// Updates the given <see cref="ProductImage"/>.
        /// </summary>
        /// <param name="productId">The id of the product that counted images belong to.</param>
        /// <param name="productImageId">Id of the object being updated.</param>
        /// <param name="inputObject">The <see cref="ProductImage"/> to update.</param>
        /// <returns>The updated <see cref="ProductImage"/>.</returns>
        public virtual async Task<ProductImage> UpdateAsync(long productId, long productImageId, ProductImage inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"image", inputObject}
            };
            return
                await
                    MakeRequestAsync<ProductImage>($"products/{productId}/images/{productImageId}.json", HttpMethod.Put,
                        "image", root);
        }

        /// <summary>
        /// Deletes a ProductImage with the given Id.
        /// </summary>
        /// <param name="productId">The id of the product that counted images belong to.</param>
        /// <param name="imageId">The ProductImage object's Id.</param>
        public virtual async Task DeleteAsync(long productId, long imageId)
        {
            await MakeRequestAsync($"products/{productId}/images/{imageId}.json", HttpMethod.Delete);
        }
    }
}
