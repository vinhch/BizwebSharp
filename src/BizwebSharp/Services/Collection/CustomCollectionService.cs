using BizwebSharp.Infrastructure;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating mapping between custom collections and collections
    /// </summary>
    public class CustomCollectionService : CollectionService<CustomCollection>
    {
        public CustomCollectionService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
