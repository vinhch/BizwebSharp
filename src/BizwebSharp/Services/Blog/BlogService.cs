using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Extensions;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class BlogService : BaseService
    {
        public BlogService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(BlogOption options = null)
        {
            return await MakeRequest<int>("blogs/count.json", HttpMethod.GET, "count", options);
        }

        public virtual async Task<IEnumerable<Blog>> ListAsync(BlogOption options = null)
        {
            return await MakeRequest<List<Blog>>("blogs.json", HttpMethod.GET, "blogs", options);
        }

        public virtual async Task<Blog> GetAsync(long id, string fields = null)
        {
            dynamic option = null;
            if (!string.IsNullOrEmpty(fields))
            {
                option = new { fields };
            }

            return await MakeRequest<Blog>($"blogs/{id}.json", HttpMethod.GET, "blog", option);
        }

        public virtual async Task<Blog> CreateAsync(Blog blog, IEnumerable<MetaField> metafields = null)
        {
            var body = blog.ToDictionary();

            if (metafields != null && metafields.Any())
            {
                body["metafields"] = metafields;
            }

            return await MakeRequest<Blog>($"blogs.json", HttpMethod.POST, "blog", new {blog = body});
        }

        public virtual async Task<Blog> UpdateAsync(long blogId, Blog blog, IEnumerable<MetaField> metafields = null)
        {
            var body = blog.ToDictionary();

            if (metafields != null && metafields.Any())
            {
                body["metafields"] = metafields;
            }

            return await MakeRequest<Blog>($"blogs/{blogId}.json", HttpMethod.PUT, "blog", new {blog = body});
        }

        public virtual async Task DeleteAsync(long id)
        {
            await MakeRequest($"blogs/{id}.json", HttpMethod.DELETE);
        }
    }
}
