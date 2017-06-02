using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Extensions;
using BizwebSharp.Infrastructure;

namespace BizwebSharp.Services
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

            return await MakeRequest<Product>($"products.json", HttpMethod.POST, "product", new { product = productBody });
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

        public virtual async Task<Product> UnpublishAsync(long id)
        {
            return await PublishAsync(id, false);
        }
    }
}