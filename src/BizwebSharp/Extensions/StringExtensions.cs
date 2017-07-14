using System.Linq;

namespace BizwebSharp
{
    public static class StringExtensions
    {
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
