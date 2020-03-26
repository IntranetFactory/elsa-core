using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elsa.ExpressionTypes
{
    public static class StringExtensions
    {
        public static T Parse<T>(this string value) => (T)value.Parse(typeof(T));

        public static object Parse(this string value, Type targetType)
        {
            if (typeof(string) == targetType || targetType == default)
                return value;

            var converter = TypeDescriptor.GetConverter(targetType);
            return converter.ConvertFromString(value);
        }
    }
}
