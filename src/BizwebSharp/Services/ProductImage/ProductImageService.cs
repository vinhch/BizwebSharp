using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class ProductImageService : BaseService
    {
        public ProductImageService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(long productId, PublishableListOption option = null)
        {
            return await MakeRequest<int>($"products/{productId}/images/count.json", HttpMethod.GET, "count", option);
        }

        public virtual async Task<IEnumerable<ProductImage>> ListAsync(long productId, PublishableListOption option = null)
        {
            return
                await
                    MakeRequest<List<ProductImage>>($"products/{productId}/images.json", HttpMethod.GET, "images",
                        option);
        }

        public virtual async Task<ProductImage> GetAsync(long productId, long imageId, string fields = null)
        {
            dynamic options = null;
            if (!string.IsNullOrEmpty(fields))
            {
                options = new { fields };
            }

            return
                await
                    MakeRequest<ProductImage>($"products/{productId}/images/{imageId}.json", HttpMethod.GET, "image",
                        options);
        }

        public virtual async Task<ProductImage> CreateAsync(long productId, ProductImage inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"image", inputObject}
            };

            return await MakeRequest<ProductImage>($"products/{productId}/images.json", HttpMethod.POST, "image", root);
        }

        public virtual async Task<ProductImage> UpdateAsync(long productId, long productImageId, ProductImage inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"image", inputObject}
            };
            return
                await
                    MakeRequest<ProductImage>($"products/{productId}/images/{productImageId}.json", HttpMethod.PUT,
                        "image", root);
        }

        public virtual async Task DeleteAsync(long productId, long imageId)
        {
            await MakeRequest($"products/{productId}/images/{imageId}.json", HttpMethod.DELETE);
        }
    }
}
