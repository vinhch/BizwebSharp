using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Extensions;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class ArticleService : BaseService
    {
        public ArticleService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(long blogId, ArticleOptions options = null)
        {
            return await MakeRequest<int>($"blogs/{blogId}/articles/count.json", HttpMethod.GET, "count", options);
        }

        public virtual async Task<IEnumerable<Article>> ListAsync(long blogId, ArticleOptions options = null)
        {
            return
                await MakeRequest<List<Article>>($"blogs/{blogId}/articles.json", HttpMethod.GET, "articles", options);
        }

        public virtual async Task<Article> GetAsync(long articleId, string fields = null)
        {
            return await MakeRequest<Article>($"articles/{articleId}.json", HttpMethod.GET, "article", new {fields});
        }

        public virtual async Task<Article> GetAsync(long blogId, long articleId, string fields = null)
        {
            return
                await
                    MakeRequest<Article>($"blogs/{blogId}/articles/{articleId}.json", HttpMethod.GET, "article",
                        new {fields});
        }

        public virtual async Task<Article> CreateAsync(long blogId, Article article,
            IEnumerable<MetaField> metafields = null)
        {
            var body = article.ToDictionary();

            if (metafields != null && metafields.Any())
            {
                body.Add("metafields", metafields);
            }

            return
                await
                    MakeRequest<Article>($"blogs/{blogId}/articles.json", HttpMethod.POST, "article",
                        new {article = body});
        }

        public virtual async Task<Article> UpdateAsync(long blogId, long articleId, Article article,
            IEnumerable<MetaField> metafields = null)
        {
            var body = article.ToDictionary();
            if (metafields != null)
            {
                body.Add("metafields", metafields);
            }

            return
                await
                    MakeRequest<Article>($"blogs/{blogId}/articles/{articleId}.json", HttpMethod.PUT, "article",
                        new {article = body});
        }

        public virtual async Task DeleteAsync(long blogId, long articleId)
        {
            await MakeRequest($"blogs/{blogId}/articles/{articleId}.json", HttpMethod.DELETE);
        }

        public virtual async Task<IEnumerable<string>> ListAuthorsAsync()
        {
            return await MakeRequest<List<string>>($"articles/authors.json", HttpMethod.GET, "authors");
        }

        public virtual async Task<IEnumerable<string>> ListTagsAsync(int? popular = null, int? limit = null)
        {
            var options = CreateListTagsOptions(popular, limit);
            return await MakeRequest<List<string>>($"articles/tags.json", HttpMethod.GET, "tags", options);
        }

        public virtual async Task<IEnumerable<string>> ListTagsForBlogAsync(long blogId, int? popular = null, int? limit = null)
        {
            var options = CreateListTagsOptions(popular, limit);
            return await MakeRequest<List<string>>($"blogs/{blogId}/articles/tags.json", HttpMethod.GET, "tags", options);
        }

        private static Dictionary<string, object> CreateListTagsOptions(int? popular, int? limit)
        {
            var options = new Dictionary<string, object>();

            if (popular.HasValue)
            {
                options.Add("popular", popular.Value);
            }

            if (limit.HasValue)
            {
                options.Add("limit", limit.Value);
            }

            return options.Any() ? options : null;
        }
    }
}
