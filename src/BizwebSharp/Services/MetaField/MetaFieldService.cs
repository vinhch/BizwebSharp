using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
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
                    MakeRequestAsync<int>(
                        $"{resourceType}/{resourceId}/{ApiClassPathInPlural}/count.json",
                        HttpMethod.Get, "count", option);
        }

        public virtual async Task<IEnumerable<MetaField>> ListAsync(long resourceId, string resourceType,
            MetaFieldOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<MetaField>>($"{resourceType}/{resourceId}/{ApiClassPathInPlural}.json",
                        HttpMethod.Get, ApiClassPathInPlural, option);
        }

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
