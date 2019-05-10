using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Themes API.
    /// </summary>
    public class ThemeService : BaseServiceWithSimpleCRUD<Theme, ListOption>
    {
        public ThemeService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Creates a new theme on the store. The theme always starts out with a role of
        /// "unpublished." If the theme has a different role, it will be assigned that only after all of its
        /// files have been extracted and stored by Bizweb (which might take a couple of minutes).
        /// </summary>
        /// <param name="theme">The new theme.</param>
        /// <param name="sourceUrl">A URL that points to the .zip file containing the theme's source files.</param>
        public virtual async Task<Theme> CreateAsync(Theme theme, string sourceUrl)
        {
            var body = theme.ToDictionary();
            if (!string.IsNullOrEmpty(sourceUrl))
            {
                body["src"] = sourceUrl;
            }

            var root = new Dictionary<string, object>
            {
                {ApiClassPath, body}
            };

            return await MakeRequestAsync<Theme>($"{ApiClassPathInPlural}.json", HttpMethod.Post, ApiClassPath, root);
        }
    }
}
