#if (NET45 || NET451 || NET452 || NET46 || NET461 || NET462)
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace BizwebSharp
{
    /// <summary>
    /// Collection extension methods
    /// </summary>
    internal static class CollectionExtensions
    {
        /// <summary>
        /// Convert NameValueCollection to KeyValuePair enumerable
        /// </summary>
        public static IEnumerable<KeyValuePair<string, string>> ToPairs(this NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return collection.Cast<string>().Select(key => new KeyValuePair<string, string>(key, collection[key]));
        }

        /// <summary>
        /// Convert NameValueCollection to KeyValuePair enumerable
        /// </summary>
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
#endif