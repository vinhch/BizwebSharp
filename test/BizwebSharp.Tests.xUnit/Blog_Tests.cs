using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Enums;
using BizwebSharp.Infrastructure;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Blog")]
    public class Blog_Tests : IClassFixture<Blog_Tests_Fixture>
    {
        private Blog_Tests_Fixture Fixture { get; }

        public Blog_Tests(Blog_Tests_Fixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact(DisplayName = "Count Blogs")]
        public async Task Counts_Blogs()
        {
            var count = await Fixture.Service.CountAsync();

            Assert.True(count > 0);
        }

        [Fact(DisplayName = "List Blogs")]
        public async Task Lists_Blogs()
        {
            var list = await Fixture.Service.ListAsync();

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Get Blogs")]
        public async Task Gets_Blogs()
        {
            var id = Fixture.Created.First().Id.Value;
            var blog = await Fixture.Service.GetAsync(id);

            Assert.True(blog.Id.HasValue);
            Assert.StartsWith(Fixture.Name, blog.Name);
            Assert.Equal(blog.Commentable, Fixture.Commentable);
        }

        [Fact(DisplayName = "Delete Blogs")]
        public async Task Deletes_Blogs()
        {
            var created = await Fixture.Create(true);
            bool threw = false;

            try
            {
                await Fixture.Service.DeleteAsync(created.Id.Value);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_Blogs)} threw exception. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }

        [Fact(DisplayName = "Create Blogs")]
        public async Task Creates_Blogs()
        {
            var created = await Fixture.Create();

            Assert.NotNull(created);
            Assert.StartsWith(Fixture.Name, created.Name);
            Assert.Equal(Fixture.Commentable, created.Commentable);
        }

        [Fact(DisplayName = "Update Blogs")]
        public async Task Updates_Blogs()
        {
            var created = await Fixture.Create();
            long id = created.Id.Value;

            created.Commentable = BlogCommentable.Yes;
            created.Id = null;

            var updated = await Fixture.Service.UpdateAsync(id, created);

            // Reset the id so the Fixture can properly delete this object.
            created.Id = id;

            Assert.Equal(BlogCommentable.Yes, updated.Commentable);
        }
    }

    public class Blog_Tests_Fixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            // Create one blog for methods like count, get, list, etc.
            await Create();
        }

        public async Task DisposeAsync()
        {
            foreach (var obj in Created)
            {
                try
                {
                    await Service.DeleteAsync(obj.Id.Value);
                }
                catch (BizwebSharpException ex)
                {
                    if (ex.HttpStatusCode != HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Failed to delete created Blog with id {obj.Id.Value}. {ex.Message}");
                    }
                }
            }
        }

        public BlogService Service => new BlogService(Utils.AuthState);

        public List<Blog> Created { get; } = new List<Blog>();

        public string Name => "BizwebSharp Test Blog";

        public BlogCommentable Commentable => BlogCommentable.Moderate;

        public async Task<Blog> Create(bool skipAddToCreatedList = false)
        {
            var blog = await Service.CreateAsync(new Blog
            {
                Name = $"{Name} #{Guid.NewGuid()}",
                Commentable = Commentable,
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(blog);
            }

            return blog;
        }
    }
}
