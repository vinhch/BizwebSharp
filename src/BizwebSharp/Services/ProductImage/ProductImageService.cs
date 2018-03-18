using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class ProductImageService : BaseService
    {
        public ProductImageService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(long productId, PublishableListOption option = null)
        {
            return await MakeRequestAsync<int>($"products/{productId}/images/count.json", HttpMethod.Get, "count", option);
        }

        public virtual async Task<IEnumerable<ProductImage>> ListAsync(long productId, PublishableListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<ProductImage>>($"products/{productId}/images.json", HttpMethod.Get, "images",
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
                    MakeRequestAsync<ProductImage>($"products/{productId}/images/{imageId}.json", HttpMethod.Get, "image",
                        options);
        }

        public virtual async Task<ProductImage> CreateAsync(long productId, ProductImage inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"image", inputObject}
            };

            return await MakeRequestAsync<ProductImage>($"products/{productId}/images.json", HttpMethod.Post, "image", root);
        }

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

        public virtual async Task DeleteAsync(long productId, long imageId)
        {
            await MakeRequestAsync($"products/{productId}/images/{imageId}.json", HttpMethod.Delete);
        }
    }
}
