using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating ScriptTag API.
    /// </summary>
    public class ScriptTagService : BaseServiceWithSimpleCRUD<ScriptTag, ScriptTagOption>
    {
        public ScriptTagService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
