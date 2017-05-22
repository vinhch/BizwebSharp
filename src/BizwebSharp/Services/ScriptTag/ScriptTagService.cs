using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class ScriptTagService : BaseServiceWithSimpleCRUD<ScriptTag, ScriptTagOption>
    {
        public ScriptTagService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
