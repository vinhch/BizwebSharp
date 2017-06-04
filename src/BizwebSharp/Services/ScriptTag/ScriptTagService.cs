﻿using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class ScriptTagService : BaseServiceWithSimpleCRUD<ScriptTag, ScriptTagOption>
    {
        public ScriptTagService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
