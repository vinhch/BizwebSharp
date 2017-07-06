using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace BizwebSharp
{
    internal static class CollectionExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> ToPairs(this NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return collection.Cast<string>().Select(key => new KeyValuePair<string, string>(key, collection[key]));
        }

        public static IEnumerable<KeyValuePair<string, StringValues>> ToPairs2(this NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return collection.Cast<string>().Select(key => new KeyValuePair<string, StringValues>(key, collection[key]));
        }
    }
}
