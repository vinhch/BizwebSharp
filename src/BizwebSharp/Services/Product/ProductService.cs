using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    public class ProductService : BaseServiceWithSimpleCRUD<Product, Options.ProductOption>
    {
        public ProductService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<Product> CreateAsync(Product product, ProductCreateOption option)
        {
            //Build the request body as a dictionary. Necessary because the create options must be added to the
            //'product' property.
            var productBody = product.ToDictionary();

            if (option != null)
            {
                foreach (var kvp in option.ToDictionary())
                {
                    productBody[kvp.Key] = kvp.Value;
                }
            }

            return await MakeRequestAsync<Product>($"products.json", HttpMethod.Post, "product", new { product = productBody });
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

            return await MakeRequestAsync<Product>($"products/{id}.json", HttpMethod.Put, "product", productBody);
        }

        public virtual async Task<Product> UnpublishAsync(long id)
        {
            return await PublishAsync(id, false);
        }
    }
}