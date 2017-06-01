using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;

namespace BizwebSharp.Services
{
    public class AssetService : BaseService
    {
        public AssetService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<IEnumerable<Asset>> ListAsync(long themeId, string fields = null)
        {
            return
                await MakeRequest<List<Asset>>($"themes/{themeId}/assets.json", HttpMethod.GET, "assets", new {fields});
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

            return await MakeRequest<Asset>($"themes/{themeId}/assets.json", HttpMethod.GET, "asset", option);
        }

        public virtual async Task<Asset> CreateOrUpdateAsync(long themeId, Asset asset)
        {
            return await MakeRequest<Asset>($"themes/{themeId}/assets.json", HttpMethod.PUT, "asset", new {asset});
        }

        public virtual async Task DeleteAsync(long themeId, string key)
        {
            var option = new Dictionary<string, object>
            {
                {"key", key },
                {"theme_id", themeId }
            };

            await MakeRequest($"themes/{themeId}/assets.json", HttpMethod.DELETE, payload: option);
        }
    }
}
