using System.Linq;

namespace BizwebSharp
{
    /// <summary>
    /// String extension methods
    /// </summary>
    internal static class StringExtensions
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

        /// <summary>
        /// Checks if a string starts with another string, ignoring case.
        /// </summary>
        public static bool StartsWithIgnoreCase(this string str, string a) => str.ToLower().StartsWith(a.ToLower());

        /// <summary>
        /// Checks if a string ends with another string, ignoring case.
        /// </summary>
        public static bool EndsWithIgnoreCase(this string str, string a) => str.ToLower().EndsWith(a.ToLower());

        /// <summary>
        /// Checks if a string contains another string, ignorning case.
        /// </summary>
        public static bool ContainsIgnoreCase(this string str, string a) => str.ToLower().Contains(a.ToLower());
    }
}
