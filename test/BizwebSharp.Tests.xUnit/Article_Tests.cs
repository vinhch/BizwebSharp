using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Services;
using Xunit;
using System.Linq;
using System.Net;
using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;
using FluentAssertions;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Article")]
    public class Article_Tests : IClassFixture<Article_Tests_Fixture>
    {
        private Article_Tests_Fixture Fixture { get; }

        public Article_Tests(Article_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Articles")]
        public async Task Count_Articles()
        {
            var count = await Fixture.Service.CountAsync(Fixture.BlogId.Value);

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Articles")]
        public async Task List_Articles()
        {
            var articles = await Fixture.Service.ListAsync(Fixture.BlogId.Value);

            Assert.True(articles.Count() > 0);
        }

        [Fact(DisplayName = "Create Articles")]
        public async Task Create_Articles()
        {
            var article = await Fixture.Create();

            Assert.True(article.Id.HasValue);
            Assert.Equal(Fixture.Author, article.Author);
            Assert.Equal(Fixture.Content, article.Content);
            Assert.Equal(Fixture.BlogId, article.BlogId);
            Assert.StartsWith(Fixture.Title, article.Title);

            article.Alias.Should().NotBeNullOrEmpty();
            article.Tags.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Update Articles")]
        public async Task Update_Articles()
        {
            const string html = "<h1>Updated!</h1>";
            var article = await Fixture.Create();
            var id = article.Id.Value;

            article.Content = html;
            article.Id = null;

            article = await Fixture.Service.UpdateAsync(Fixture.BlogId.Value, id, article);

            // Reset the id so the Fixture can properly delete this object.
            article.Id = id;

            Assert.Equal(article.Content, html);
        }

        [Fact(DisplayName = "Delete Articles")]
        public async Task Deletes_Articles()
        {
            var article = await Fixture.Create(true);
            var threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(Fixture.BlogId.Value, article.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_Articles)} threw exception. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "List Authors")]
        public async Task List_Authors()
        {
            var authors = await Fixture.Service.ListAuthorsAsync();

            Assert.True(authors.Count() > 0);
            Assert.True(authors.Any(a => a == Fixture.Author));
        }

        [Fact(DisplayName = "List Tags")]
        public async Task List_Tags()
        {
            var tags = await Fixture.Service.ListTagsAsync();

            Assert.True(tags.Count() > 0);
        }

        [Fact(DisplayName = "List Tags For Blog")]
        public async Task Lists_Tags_For_Blog()
        {
            var tags = await Fixture.Service.ListTagsForBlogAsync(Fixture.BlogId.Value);

            Assert.True(tags.Count() > 0);
        }
    }

    public class Article_Tests_Fixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            var blogService = new BlogService(Utils.AuthState);
            var blogs = await blogService.ListAsync();

            BlogId = blogs.First().Id;
        }

        public async Task DisposeAsync()
        {
            foreach (var article in Created)
            {
                try
                {
                    await Service.DeleteAsync(BlogId.Value, article.Id.Value);
                }
                catch (BizwebSharpException ex)
                {
                    if (ex.HttpStatusCode != HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Failed to delete Article with id {article.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public long? BlogId { get; set; }

        public List<Article> Created { get; } = new List<Article>();

        public ArticleService Service => new ArticleService(Utils.AuthState);

        public string Title => "My new Article title - ";

        public string Author => "John Smith";

        public string Tags => "This Post, Has Been Tagged";

        public string Content => "<h1>I like articles</h1>\n<p><strong>Yea</strong>, I like posting them through <span class=\"caps\">REST</span>.</p>";

        public async Task<Article> Create(bool skipAddToDeleteList = false)
        {
            var obj = await Service.CreateAsync(BlogId.Value, new Article
            {
                Title = Title + Guid.NewGuid(),
                Author = Author,
                Tags = Tags,
                Content = Content,
                Image = new Image
                {
                    Base64 = "R0lGODlhAQABAIAAAAAAAAAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\n"
                }
            });

            if (!skipAddToDeleteList)
            {
                Created.Add(obj);
            }

            return obj;
        }
    }
}
