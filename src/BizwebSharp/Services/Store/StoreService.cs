using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    public class StoreService : BaseService
    {
        public StoreService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<Store> GetAsync(string fields = null)
        {
            dynamic option = null;
            if (fields != null)
            {
                option = new { fields };
            }

            return await MakeRequestAsync<Store>("store.json", HttpMethod.Get, "store", option);
        }

        public virtual async Task UninstallAppAsync()
        {
            await MakeRequestAsync("api_permissions/current.json", HttpMethod.Delete);
        }
    }
}
