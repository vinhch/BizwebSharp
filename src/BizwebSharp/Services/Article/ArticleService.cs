using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating a blog's articles.
    /// </summary>
    public class ArticleService : BaseService
    {
        /// <summary>
        /// Creates a new instance of the service.
        /// </summary>
        /// <param name="authState">The object contain Bizweb authorization data.</param>
        public ArticleService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Gets a count of all articles belonging to the given blog.
        /// </summary>
        /// <param name="blogId">The blog that the articles belong to.</param>
        /// <param name="options">Optional Count API setting.</param>
        public virtual async Task<int> CountAsync(long blogId, ArticleOption options = null)
        {
            return await MakeRequestAsync<int>($"blogs/{blogId}/articles/count.json", HttpMethod.Get, "count", options);
        }

        /// <summary>
        /// Gets a list of up to 250 articles belonging to the given blog.
        /// </summary>
        /// <param name="blogId">The blog that the articles belong to.</param>
        /// <param name="options">Optional List API setting.</param>
        public virtual async Task<IEnumerable<Article>> ListAsync(long blogId, ArticleOption options = null)
        {
            return
                await MakeRequestAsync<List<Article>>($"blogs/{blogId}/articles.json", HttpMethod.Get, "articles", options);
        }

        /// <summary>
        /// Gets an article with the given id.
        /// </summary>
        /// <param name="articleId">The article's id.</param>
        /// <param name="fields">A comma-separated list of fields to return.</param>
        public virtual async Task<Article> GetAsync(long articleId, string fields = null)
        {
            dynamic option = null;
            if (!string.IsNullOrEmpty(fields))
            {
                option = new { fields };
            }

            return await MakeRequestAsync<Article>($"articles/{articleId}.json", HttpMethod.Get, "article", option);
        }

        /// <summary>
        /// Gets an article with the given id.
        /// </summary>
        /// <param name="blogId">The blog that the article belongs to.</param>
        /// <param name="articleId">The article's id.</param>
        /// <param name="fields">A comma-separated list of fields to return.</param>
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

        /// <summary>
        /// Creates a new article on the given blog.
        /// </summary>
        /// <param name="blogId">The blog that the article will belong to.</param>
        /// <param name="article">The article being created. Id should be null.</param>
        /// <param name="metafields">Optional metafield data that can be returned by the <see cref="MetaFieldService"/>.</param>
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

        /// <summary>
        /// Updates an article.
        /// </summary>
        /// <param name="blogId">The blog that the article belongs to.</param>
        /// <param name="article">The article being updated. Id should not be null.</param>
        /// <param name="metafields">Optional metafield data that can be returned by the <see cref="MetaFieldService"/>.</param>
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

        /// <summary>
        /// Deletes an article with the given id.
        /// </summary>
        /// <param name="blogId">The blog that the article belongs to.</param>
        /// <param name="articleId">The article benig deleted.</param>
        public virtual async Task DeleteAsync(long blogId, long articleId)
        {
            await MakeRequestAsync($"blogs/{blogId}/articles/{articleId}.json", HttpMethod.Delete);
        }

        /// <summary>
        /// Gets a list of all article authors.
        /// </summary>
        public virtual async Task<IEnumerable<string>> ListAuthorsAsync()
        {
            return await MakeRequestAsync<List<string>>($"articles/authors.json", HttpMethod.Get, "authors");
        }

        /// <summary>
        /// Gets a list of all article tags.
        /// </summary>
        /// <param name="popular">A flag to indicate only to a certain number of the most popular tags.</param>
        /// <param name="limit">The number of tags to return</param>
        public virtual async Task<IEnumerable<string>> ListTagsAsync(int? popular = null, int? limit = null)
        {
            var options = CreateListTagsOptions(popular, limit);
            return await MakeRequestAsync<List<string>>($"articles/tags.json", HttpMethod.Get, "tags", options);
        }

        /// <summary>
        /// Gets a list of all article tags for the given blog.
        /// </summary>
        /// <param name="blogId">The blog that the tags belong to.</param>
        /// <param name="popular">A flag to indicate only to a certain number of the most popular tags.</param>
        /// <param name="limit">The number of tags to return</param>
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
