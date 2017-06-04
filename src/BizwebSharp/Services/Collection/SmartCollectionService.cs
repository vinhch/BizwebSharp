using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    public class SmartCollectionService : CollectionService<SmartCollection>
    {
        public SmartCollectionService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
