using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Store info API.
    /// </summary>
    public class StoreService : BaseService
    {
        public StoreService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Gets the shop's data.
        /// </summary>
        public virtual async Task<Store> GetAsync(string fields = null)
        {
            var option = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(fields))
            {
                option["fields"] = fields;
            }

            return await MakeRequestAsync<Store>("store.json", HttpMethod.Get, "store", option);
        }

        /// <summary>
        /// Forces the shop to uninstall your app. Uninstalling an application is an irreversible operation.
        /// Be entirely sure that you no longer need to make API calls for the shop in which the application has been installed.
        /// </summary>
        public virtual async Task UninstallAppAsync()
        {
            await MakeRequestAsync("api_permissions/current.json", HttpMethod.Delete);
        }
    }
}
