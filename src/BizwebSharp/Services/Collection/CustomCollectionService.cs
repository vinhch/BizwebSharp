using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;

namespace BizwebSharp.Services
{
    public class CustomCollectionService : CollectionService<CustomCollection>
    {
        public CustomCollectionService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
