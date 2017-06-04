using BizwebSharp.Infrastructure;

namespace BizwebSharp.Services
{
    public class SmartCollectionService : CollectionService<SmartCollection>
    {
        public SmartCollectionService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
