using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Assets API.
    /// </summary>
    public class AssetService : BaseService
    {
        /// <summary>
        /// Creates a new instance of the service.
        /// </summary>
        /// <param name="authState">The object contain Bizweb authorization data.</param>
        public AssetService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Retrieves a list of all <see cref="Asset"/> objects. Listing theme assets only returns metadata about each asset.
        /// You need to request assets individually in order to get their contents.
        /// </summary>
        /// <param name="themeId">The id of the theme that the asset belongs to.</param>
        /// <param name="fields">A comma-separated list of fields to return.</param>
        /// <returns>The list of <see cref="Asset"/> objects.</returns>
        public virtual async Task<IEnumerable<Asset>> ListAsync(long themeId, string fields = null)
        {
            return
                await MakeRequestAsync<List<Asset>>($"themes/{themeId}/assets.json", HttpMethod.Get, "assets", new {fields});
        }

        /// <summary>
        /// Retrieves the <see cref="Asset"/> with the given id.
        /// </summary>
        /// <param name="themeId">The id of the theme that the asset belongs to. Assets themselves do not have ids.</param>
        /// <param name="key">The key value of the asset, e.g. 'templates/index.liquid' or 'assets/bg-body.gif'.</param>
        /// <param name="fields">A comma-separated list of fields to return.</param>
        /// <returns>The <see cref="Asset"/>.</returns>
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

        /// <summary>
        /// Creates or updates a <see cref="Asset"/>. Both tasks use the same method due to the
        /// way Bizweb API handles assets. If an asset has a unique <see cref="Asset.Key"/> value,
        /// it will be created. If not, it will be updated. Copy an asset by setting the
        /// <see cref="Asset.SourceKey"/> to the target's <see cref="Asset.Key"/> value.
        /// Note: This will not return the asset's <see cref="Asset.Value"/> property. You should
        /// use <see cref="GetAsync(long, string, string)"/> to refresh the value after creating or updating.
        /// </summary>
        /// <param name="themeId">The id of the theme that the asset belongs to.</param>
        /// <param name="asset">The asset.</param>
        /// <returns>The created or updated asset.</returns>
        public virtual async Task<Asset> CreateOrUpdateAsync(long themeId, Asset asset)
        {
            return await MakeRequestAsync<Asset>($"themes/{themeId}/assets.json", HttpMethod.Put, "asset", new {asset});
        }

        /// <summary>
        /// Deletes a <see cref="Asset"/> with the given key.
        /// </summary>
        /// <param name="themeId">The id of the theme that the asset belongs to.</param>
        /// <param name="key">The key value of the asset, e.g. 'templates/index.liquid' or 'assets/bg-body.gif'.</param>
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
