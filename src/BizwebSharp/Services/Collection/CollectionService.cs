using System;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Collections API.
    /// </summary>
    [Obsolete("This service is deprecated, please use CustomCollectionService or SmartCollectionService instead.")]
    public class CollectionService : CollectionService<Collection>
    {
        public CollectionService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }

    public abstract class CollectionService<T> : BaseServiceWithSimpleCRUD<T, CollectionOption>
        where T : BaseEntityWithTimeline, new()
    {
        protected CollectionService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}