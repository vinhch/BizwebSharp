using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Extensions;
using BizwebSharp.Options;
using BizwebSharp.Infrastructure;

namespace BizwebSharp.Services
{
    public class ProductService : BaseService
    {
        public ProductService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(ProductOptions options = null)
        {
            return await MakeRequest<int>("products/count.json", HttpMethod.GET, "count", options);
        }

        public virtual async Task<IEnumerable<Product>> ListAsync(ProductOptions options = null)
        {
            return await MakeRequest<List<Product>>("products.json", HttpMethod.GET, "products", options);
        }

        public virtual async Task<Product> GetAsync(long productId, string fields = null)
        {
            return await MakeRequest<Product>($"products/{productId}.json", HttpMethod.GET, "product", new { fields });
        }

        public virtual async Task<Product> CreateAsync(Product product, ProductCreateOptions options = null)
        {
            //Build the request body as a dictionary. Necessary because the create options must be added to the
            //'product' property.
            var productBody = product.ToDictionary();

            if (options != null)
            {
                foreach (var kvp in options.ToDictionary())
                {
                    productBody.Add(kvp);
                }
            }

            return await MakeRequest<Product>($"products.json", HttpMethod.POST, "product", new { product = productBody });
        }

        public virtual async Task<Product> UpdateAsync(Product product)
        {
            return await MakeRequest<Product>($"products/{product.Id.Value}.json", HttpMethod.PUT, "product", new { product });
        }

        public virtual async Task DeleteAsync(long productId)
        {
            await MakeRequest($"products/{productId}.json", HttpMethod.DELETE);
        }

        public virtual async Task<Product> PublishAsync(long id, bool isPublish = true)
        {
            var productBody = new
            {
                product = new
                {
                    id,
                    published = isPublish
                }
            };

            return await MakeRequest<Product>($"products/{id}.json", HttpMethod.PUT, "product", productBody);
        }
    }
}