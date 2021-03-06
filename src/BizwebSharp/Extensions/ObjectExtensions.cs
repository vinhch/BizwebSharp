﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace BizwebSharp
{
    internal static class ObjectExtensions
    {
        /// <summary>
        ///     Converts the object to a dictionary./>
        /// </summary>
        /// <returns>The object as a <see cref="IDictionary{String, Object}" />.</returns>
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            IDictionary<string, object> output = new Dictionary<string, object>();

            //Inspiration for this code from https://github.com/jaymedavis/stripe.net
            //foreach (
            //    var property in obj.GetType().GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //{
            foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(obj, null);
                var propName = property.Name;
                if (value == null) continue;

                if (property.CustomAttributes.Any(x => x.AttributeType == typeof(JsonPropertyAttribute)))
                {
                    //Get the JsonPropertyAttribute for this property, which will give us its JSON name
                    var attribute =
                        property.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                            .Cast<JsonPropertyAttribute>()
                            .FirstOrDefault();

                    propName = attribute != null ? attribute.PropertyName : property.Name;
                }

                if (value.GetType().GetTypeInfo().IsEnum)
                    value = ((Enum) value).ToSerializedString();

                output[propName] = value;
            }

            return output;
        }
    }
}