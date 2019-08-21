using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Smart Collections API.
    /// </summary>
    public class SmartCollectionService : CollectionService<SmartCollection>
    {
        public SmartCollectionService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
