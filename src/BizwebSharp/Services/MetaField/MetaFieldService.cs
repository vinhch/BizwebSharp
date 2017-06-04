using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class MetaFieldService : BaseServiceWithSimpleCRUD<MetaField, MetaFieldOption>
    {
        public MetaFieldService(BizwebAuthorizationState authState) : base(authState, "metafield", "metafields")
        {
        }

        public virtual async Task<int> CountAsync(long resourceId, string resourceType, MetaFieldOption option = null)
        {
            return
                await
                    MakeRequest<int>(
                        $"{resourceType}/{resourceId}/{ApiClassPathInPlural}/count.json",
                        HttpMethod.GET, "count", option);
        }

        public virtual async Task<IEnumerable<MetaField>> ListAsync(long resourceId, string resourceType,
            MetaFieldOption option = null)
        {
            return
                await
                    MakeRequest<List<MetaField>>($"{resourceType}/{resourceId}/{ApiClassPathInPlural}.json",
                        HttpMethod.GET, ApiClassPathInPlural, option);
        }

        public virtual async Task<MetaField> CreateAsync(long resourceId, string resourceType, MetaField inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {ApiClassPath, inputObject}
            };

            return
                await
                    MakeRequest<MetaField>($"{resourceType}/{resourceId}/{ApiClassPathInPlural}.json", HttpMethod.POST,
                        ApiClassPath, root);
        }
    }
}
