﻿using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class CollectService : BaseServiceWithSimpleCRUD<Collect, CollectOption>
    {
        public CollectService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
