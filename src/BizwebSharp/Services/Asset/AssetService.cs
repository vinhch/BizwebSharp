using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    public class AssetService : BaseService
    {
        public AssetService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<IEnumerable<Asset>> ListAsync(long themeId, string fields = null)
        {
            return
                await MakeRequestAsync<List<Asset>>($"themes/{themeId}/assets.json", HttpMethod.Get, "assets", new {fields});
        }

        public virtual async Task<Asset> GetAsync(long themeId, string key, string fields = null)
        {
            var option = new Dictionary<string, object>
            {
                {"key", key },
                {"theme_id", themeId }
            };

            if (!string.IsNullOrEmpty(fields))
            {
                option["fields"] = fields;
            }

            return await MakeRequestAsync<Asset>($"themes/{themeId}/assets.json", HttpMethod.Get, "asset", option);
        }

        public virtual async Task<Asset> CreateOrUpdateAsync(long themeId, Asset asset)
        {
            return await MakeRequestAsync<Asset>($"themes/{themeId}/assets.json", HttpMethod.Put, "asset", new {asset});
        }

        public virtual async Task DeleteAsync(long themeId, string key)
        {
            var option = new Dictionary<string, object>
            {
                {"key", key },
                {"theme_id", themeId }
            };

            await MakeRequestAsync($"themes/{themeId}/assets.json", HttpMethod.Delete, payload: option);
        }
    }
}
