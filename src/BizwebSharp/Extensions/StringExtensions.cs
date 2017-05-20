using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizwebSharp.Extensions
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            return string.Concat(input.Select((x, i) =>
            {
                var s = x.ToString();

                if (!char.IsUpper(x))
                {
                    return s;
                }

                return i > 0 ? "_" + s.ToLower() : s.ToLower();
            }));
        }
    }
}
