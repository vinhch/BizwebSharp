using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class BlogService : BaseServiceWithSimpleCRUD<Blog, BlogOption>
    {
        public BlogService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
