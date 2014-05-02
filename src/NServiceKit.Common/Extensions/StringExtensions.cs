using System;
using System.Collections.Generic;
using Proxy = NServiceKit.Common.StringExtensions;

namespace NServiceKit.Common.Extensions
{
    /// <summary>A string extensions.</summary>
    [Obsolete("Use NServiceKit.Common.StringExtensions")]
    public static class StringExtensions
    {
        /// <summary>A string extension method that converts a value to an enum.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>value as a T.</returns>
        public static T ToEnum<T>(this string value)
        {
            return Proxy.ToEnum<T>(value);
        }

        /// <summary>A string extension method that converts this object to an enum or default.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">       The value to act on.</param>
        /// <param name="defaultValue">The default value.</param>
        ///
        /// <returns>The given data converted to a T.</returns>
        public static T ToEnumOrDefault<T>(this string value, T defaultValue)
        {
            return Proxy.ToEnumOrDefault<T>(value, defaultValue);
        }

        /// <summary>A string extension method that splits camel case.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string SplitCamelCase(this string value)
        {
            return Proxy.SplitCamelCase(value);
        }

        /// <summary>A string extension method that converts a camelCase to an english.</summary>
        ///
        /// <param name="camelCase">The camelCase to act on.</param>
        ///
        /// <returns>camelCase as a string.</returns>
        public static string ToEnglish(this string camelCase)
        {
            return Proxy.ToEnglish(camelCase);
        }

        /// <summary>A string extension method that query if 'value' is empty.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>true if empty, false if not.</returns>
        public static bool IsEmpty(this string value)
        {
            return Proxy.IsEmpty(value);
        }

        /// <summary>A string extension method that queries if a null or is empty.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>true if a null or is empty, false if not.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return Proxy.IsNullOrEmpty(value);
        }

        /// <summary>A string extension method that equals ignore case.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        /// <param name="other">The other.</param>
        ///
        /// <returns>true if equals ignore case, false if not.</returns>
        public static bool EqualsIgnoreCase(this string value, string other)
        {
            return Proxy.EqualsIgnoreCase(value, other);
        }

        /// <summary>A string extension method that replace first.</summary>
        ///
        /// <param name="haystack">   The haystack to act on.</param>
        /// <param name="needle">     The needle.</param>
        /// <param name="replacement">The replacement.</param>
        ///
        /// <returns>A string.</returns>
        public static string ReplaceFirst(this string haystack, string needle, string replacement)
        {
            return Proxy.ReplaceFirst(haystack, needle, replacement);
        }

        /// <summary>A string extension method that replace all.</summary>
        ///
        /// <param name="haystack">   The haystack to act on.</param>
        /// <param name="needle">     The needle.</param>
        /// <param name="replacement">The replacement.</param>
        ///
        /// <returns>A string.</returns>
        public static string ReplaceAll(this string haystack, string needle, string replacement)
        {
            return Proxy.ReplaceAll(haystack, needle, replacement);
        }

        /// <summary>A string extension method that query if 'text' contains any.</summary>
        ///
        /// <param name="text">       The text to act on.</param>
        /// <param name="testMatches">A variable-length parameters list containing test matches.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool ContainsAny(this string text, params string[] testMatches)
        {
            return Proxy.ContainsAny(text, testMatches);
        }

        /// <summary>A string extension method that safe variable name.</summary>
        ///
        /// <param name="text">The text to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string SafeVarName(this string text)
        {
            return Proxy.SafeVarName(text);
        }

        /// <summary>A List&lt;string&gt; extension method that joins.</summary>
        ///
        /// <param name="items">The items to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string Join(this List<string> items)
        {
            return Proxy.Join(items);
        }

        /// <summary>A List&lt;string&gt; extension method that joins.</summary>
        ///
        /// <param name="items">    The items to act on.</param>
        /// <param name="delimeter">The delimeter.</param>
        ///
        /// <returns>A string.</returns>
        public static string Join(this List<string> items, string delimeter)
        {
            return Proxy.Join(items, delimeter);
        }
    }
}