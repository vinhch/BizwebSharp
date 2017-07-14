﻿using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class RedirectService : BaseServiceWithSimpleCRUD<Redirect, RedirectOption>
    {
        public RedirectService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
