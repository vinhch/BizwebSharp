using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Extensions;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class ThemeService : BaseServiceWithSimpleCRUD<Theme, ListOption>
    {
        public ThemeService(BizwebAuthorizationState authState) : base(authState)
        {
        }

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

            return await MakeRequest<Theme>($"{ApiClassPathInPlural}.json", HttpMethod.POST, ApiClassPath, root);
        }
    }
}
