using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Metafields API.
    /// </summary>
    public class MetaFieldService : BaseServiceWithSimpleCRUD<MetaField, MetaFieldOption>
    {
        public MetaFieldService(BizwebAuthorizationState authState) : base(authState, "metafield", "metafields")
        {
        }

        /// <summary>
        /// Gets a count of the metafields for the given entity type and filter options.
        /// </summary>
        /// <param name="resourceType">The type of resource to obtain metafields for. This could be variants, products, orders, customers, custom_collections.</param>
        /// <param name="resourceId">The Id for the resource type.</param>
        /// <param name="option">Options to filter the results.</param>
        public virtual async Task<int> CountAsync(long resourceId, string resourceType, MetaFieldOption option = null)
        {
            return
                await
                    MakeRequestAsync<int>(
                        $"{resourceType}/{resourceId}/{ApiClassPathInPlural}/count.json",
                        HttpMethod.Get, "count", option);
        }

        /// <summary>
        /// Gets a list of the metafields for the given entity type and filter options.
        /// </summary>
        /// <param name="resourceType">The type of resource to obtain metafields for. This could be variants, products, orders, customers, custom_collections.</param>
        /// <param name="resourceId">The Id for the resource type.</param>
        /// <param name="option">Options to filter the results.</param>
        public virtual async Task<IEnumerable<MetaField>> ListAsync(long resourceId, string resourceType,
            MetaFieldOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<MetaField>>($"{resourceType}/{resourceId}/{ApiClassPathInPlural}.json",
                        HttpMethod.Get, ApiClassPathInPlural, option);
        }

        /// <summary>
        /// Creates a new metafield on the given entity.
        /// </summary>
        /// <param name="inputObject">A new metafield. Id should be set to null.</param>
        /// <param name="resourceId">The Id of the resource the metafield will be associated with. This can be variants, products, orders, customers, custom_collections, etc.</param>
        /// <param name="resourceType">The resource type the metaifeld will be associated with. This can be variants, products, orders, customers, custom_collections, etc.</param>
        public virtual async Task<MetaField> CreateAsync(long resourceId, string resourceType, MetaField inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, inputObject}
            };

            return
                await
                    MakeRequestAsync<MetaField>($"{resourceType}/{resourceId}/{ApiClassPathInPlural}.json", HttpMethod.Post,
                        ApiClassPath, root);
        }
    }
}
