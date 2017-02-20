using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace BizwebSharp.Extensions
{
    /// <summary>
    ///     Enum Extension Method
    /// </summary>
    internal static class EnumExtensions
    {
        /// <summary>
        ///     Reads and uses the enum's <see cref="EnumMemberAttribute" /> for serialization.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSerializedString(this Enum input)
        {
            var name = input.ToString();
            var info = input.GetType().GetTypeInfo().GetMember(name);

            if (info.Length > 0)
            {
                var attribute = info[0].GetCustomAttribute<EnumMemberAttribute>();

                if (attribute != null)
                    return attribute.Value;
            }

            return name.ToLower();
        }

        /// <summary>
        ///     Convert list of Enums to a comma seperated string
        /// </summary>
        public static string EnumListToString<T>(IEnumerable<T> enumList)
        {
            var list = new List<string>();

            if ((enumList != null) && enumList.Any())
                list.AddRange(enumList.Select(enumItem => ToSerializedString(enumItem as Enum)));
            return string.Join(",", list);
        }
    }
}