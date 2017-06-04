﻿using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
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