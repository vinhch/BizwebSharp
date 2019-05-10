using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Collect API, the mapping between products and collections.
    /// </summary>
    public class CollectService : BaseServiceWithSimpleCRUD<Collect, CollectOption>
    {
        public CollectService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
