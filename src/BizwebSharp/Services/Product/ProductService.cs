using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Products API.
    /// </summary>
    public class ProductService : BaseServiceWithSimpleCRUD<Product, Options.ProductOption>
    {
        public ProductService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Creates a new <see cref="Product"/> on the store.
        /// </summary>
        /// <param name="product">A new <see cref="Product"/>. Id should be set to null.</param>
        /// <param name="option">Options for creating the product.</param>
        /// <returns>The new <see cref="Product"/>.</returns>
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

        private async Task<Product> PublishProductAsync(long id, bool isPublish = true)
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

        /// <summary>
        /// Publishes an unpublished <see cref="Product"/>.
        /// </summary>
        /// <param name="id">The product's id.</param>
        /// <returns>The published <see cref="Product"/></returns>
        public virtual async Task<Product> PublishAsync(long id)
        {
            return await PublishProductAsync(id, true);
        }

        /// <summary>
        /// Unpublishes an published <see cref="Product"/>.
        /// </summary>
        /// <param name="id">The product's id.</param>
        /// <returns>The unpublished <see cref="Product"/></returns>
        public virtual async Task<Product> UnpublishAsync(long id)
        {
            return await PublishProductAsync(id, false);
        }
    }
}