using System.Linq;

namespace BizwebSharp
{
    /// <summary>
    /// String extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Convert PascalCase or camelCase to snack_case
        /// </summary>
        public static string ToSnakeCase(this string input)
        {
            return string.Concat(input.Select((x, i) =>
            {
                var s = x.ToString();
                if (char.IsLower(x))
                {
                    return s;
                }
                return i > 0 ? "_" + s.ToLower() : s.ToLower();
            }));
        }
    }
}
