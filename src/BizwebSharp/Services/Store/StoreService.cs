using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;

namespace BizwebSharp.Services
{
    public class StoreService : BaseService
    {
        public StoreService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<Store> GetAsync(string fields = null)
        {
            return await MakeRequest<Store>("store.json", HttpMethod.GET, "store", new { fields });
        }

        public virtual async Task UninstallAppAsync()
        {
            await MakeRequest("api_permissions/current.json", HttpMethod.DELETE);
        }
    }
}
