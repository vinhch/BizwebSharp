using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    public class CustomCollectionService : CollectionService<CustomCollection>
    {
        public CustomCollectionService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
