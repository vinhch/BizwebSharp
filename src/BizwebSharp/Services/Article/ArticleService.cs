using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class ArticleService : BaseService
    {
        public ArticleService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(long blogId, ArticleOption options = null)
        {
            return await MakeRequestAsync<int>($"blogs/{blogId}/articles/count.json", HttpMethod.Get, "count", options);
        }

        public virtual async Task<IEnumerable<Article>> ListAsync(long blogId, ArticleOption options = null)
        {
            return
                await MakeRequestAsync<List<Article>>($"blogs/{blogId}/articles.json", HttpMethod.Get, "articles", options);
        }

        public virtual async Task<Article> GetAsync(long articleId, string fields = null)
        {
            dynamic option = null;
            if (!string.IsNullOrEmpty(fields))
            {
                option = new { fields };
            }

            return await MakeRequestAsync<Article>($"articles/{articleId}.json", HttpMethod.Get, "article", option);
        }

        public virtual async Task<Article> GetAsync(long blogId, long articleId, string fields = null)
        {
            dynamic option = null;
            if (!string.IsNullOrEmpty(fields))
            {
                option = new { fields };
            }

            return
                await
                    MakeRequestAsync<Article>($"blogs/{blogId}/articles/{articleId}.json", HttpMethod.Get, "article", option);
        }

        public virtual async Task<Article> CreateAsync(long blogId, Article article,
            IEnumerable<MetaField> metafields = null)
        {
            var body = article.ToDictionary();

            if (metafields != null && metafields.Any())
            {
                body["metafields"] = metafields;
            }

            return
                await
                    MakeRequestAsync<Article>($"blogs/{blogId}/articles.json", HttpMethod.Post, "article",
                        new {article = body});
        }

        public virtual async Task<Article> UpdateAsync(long blogId, long articleId, Article article,
            IEnumerable<MetaField> metafields = null)
        {
            var body = article.ToDictionary();
            if (metafields != null)
            {
                body["metafields"] = metafields;
            }

            return
                await
                    MakeRequestAsync<Article>($"blogs/{blogId}/articles/{articleId}.json", HttpMethod.Put, "article",
                        new {article = body});
        }

        public virtual async Task DeleteAsync(long blogId, long articleId)
        {
            await MakeRequestAsync($"blogs/{blogId}/articles/{articleId}.json", HttpMethod.Delete);
        }

        public virtual async Task<IEnumerable<string>> ListAuthorsAsync()
        {
            return await MakeRequestAsync<List<string>>($"articles/authors.json", HttpMethod.Get, "authors");
        }

        public virtual async Task<IEnumerable<string>> ListTagsAsync(int? popular = null, int? limit = null)
        {
            var options = CreateListTagsOptions(popular, limit);
            return await MakeRequestAsync<List<string>>($"articles/tags.json", HttpMethod.Get, "tags", options);
        }

        public virtual async Task<IEnumerable<string>> ListTagsForBlogAsync(long blogId, int? popular = null, int? limit = null)
        {
            var options = CreateListTagsOptions(popular, limit);
            return await MakeRequestAsync<List<string>>($"blogs/{blogId}/articles/tags.json", HttpMethod.Get, "tags", options);
        }

        private static Dictionary<string, object> CreateListTagsOptions(int? popular, int? limit)
        {
            var options = new Dictionary<string, object>();

            if (popular.HasValue)
            {
                options["popular"] = popular.Value;
            }

            if (limit.HasValue)
            {
                options["limit"] = limit.Value;
            }

            return options.Any() ? options : null;
        }
    }
}
