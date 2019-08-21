using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for interacting with a store's blogs (not blog posts).
    /// </summary>
    public class BlogService : BaseServiceWithSimpleCRUD<Blog, BlogOption>
    {
        public BlogService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
