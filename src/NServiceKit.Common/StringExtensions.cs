using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NServiceKit.Common.Utils;
using NServiceKit.Text;
using NServiceKit.Text.Common;

namespace NServiceKit.Common
{
    /// <summary>A string extensions.</summary>
    public static partial class StringExtensions
    {

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
        private const RegexOptions PlatformRegexOptions = RegexOptions.Compiled;
#else
        private const RegexOptions PlatformRegexOptions = RegexOptions.None;
#endif

        private static readonly Regex InvalidVarCharsRegex = new Regex(@"[^A-Za-z0-9]", PlatformRegexOptions);
        private static readonly Regex SplitCamelCaseRegex = new Regex("([A-Z]|[0-9]+)", PlatformRegexOptions);
        private static readonly Regex HttpRegex = new Regex(@"^http://",
            PlatformRegexOptions | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        /// <summary>A string extension method that converts a value to an enum.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>value as a T.</returns>
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
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
            if (String.IsNullOrEmpty(value)) return defaultValue;
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>A string extension method that splits camel case.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string SplitCamelCase(this string value)
        {
            return SplitCamelCaseRegex.Replace(value, " $1").TrimStart();
        }

        /// <summary>A string extension method that converts a value to a camel case.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>value as a string.</returns>
        public static string ToCamelCase(this string value)
        {
            return Text.StringExtensions.ToCamelCase(value);
        }

        /// <summary>A string extension method that converts a value to a lowercase underscore.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>value as a string.</returns>
        public static string ToLowercaseUnderscore(this string value)
        {
            return Text.StringExtensions.ToLowercaseUnderscore(value);
        }

        /// <summary>A char extension method that converts a value to an invariant upper.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>value as a string.</returns>
        public static string ToInvariantUpper(this char value)
        {
#if NETFX_CORE
            return value.ToString().ToUpperInvariant();
#else
            return value.ToString(CultureInfo.InvariantCulture).ToUpper();
#endif
        }

        /// <summary>A string extension method that converts a camelCase to an english.</summary>
        ///
        /// <param name="camelCase">The camelCase to act on.</param>
        ///
        /// <returns>camelCase as a string.</returns>
        public static string ToEnglish(this string camelCase)
        {
            var ucWords = camelCase.SplitCamelCase().ToLower();
            return ucWords[0].ToInvariantUpper() + ucWords.Substring(1);
        }

        /// <summary>A string extension method that converts an URL to the HTTPS.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="url">The URL to act on.</param>
        ///
        /// <returns>URL as a string.</returns>
        public static string ToHttps(this string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            return HttpRegex.Replace(url.Trim(), "https://");
        }

        /// <summary>A string extension method that query if 'value' is empty.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>true if empty, false if not.</returns>
        public static bool IsEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        /// <summary>A string extension method that queries if a null or is empty.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>true if a null or is empty, false if not.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        /// <summary>A string extension method that equals ignore case.</summary>
        ///
        /// <param name="value">The value to act on.</param>
        /// <param name="other">The other.</param>
        ///
        /// <returns>true if equals ignore case, false if not.</returns>
        public static bool EqualsIgnoreCase(this string value, string other)
        {
            return String.Equals(value, other, StringComparison.CurrentCultureIgnoreCase);
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
            var pos = haystack.IndexOf(needle);
            if (pos < 0) return haystack;

            return haystack.Substring(0, pos) + replacement + haystack.Substring(pos + needle.Length);
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
            int pos;
            // Avoid a possible infinite loop
            if (needle == replacement) return haystack;
            while ((pos = haystack.IndexOf(needle)) > 0)
            {
                haystack = haystack.Substring(0, pos) 
                    + replacement 
                    + haystack.Substring(pos + needle.Length);
            }
            return haystack;
        }

        /// <summary>A string extension method that query if 'text' contains any.</summary>
        ///
        /// <param name="text">       The text to act on.</param>
        /// <param name="testMatches">A variable-length parameters list containing test matches.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool ContainsAny(this string text, params string[] testMatches)
        {
            foreach (var testMatch in testMatches)
            {
                if (text.Contains(testMatch)) return true;
            }
            return false;
        }

        /// <summary>A string extension method that safe variable name.</summary>
        ///
        /// <param name="text">The text to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string SafeVarName(this string text)
        {
            if (String.IsNullOrEmpty(text)) return null;
            return InvalidVarCharsRegex.Replace(text, "_");
        }

        /// <summary>A List&lt;string&gt; extension method that joins.</summary>
        ///
        /// <param name="items">The items to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string Join(this List<string> items)
        {
            return String.Join(JsWriter.ItemSeperatorString, items.ToArray());
        }

        /// <summary>A List&lt;string&gt; extension method that joins.</summary>
        ///
        /// <param name="items">    The items to act on.</param>
        /// <param name="delimeter">The delimeter.</param>
        ///
        /// <returns>A string.</returns>
        public static string Join(this List<string> items, string delimeter)
        {
            return String.Join(delimeter, items.ToArray());
        }

        /// <summary>A string extension method that combine with.</summary>
        ///
        /// <param name="path">      The path to act on.</param>
        /// <param name="thesePaths">A variable-length parameters list containing these paths.</param>
        ///
        /// <returns>A string.</returns>
        public static string CombineWith(this string path, params string[] thesePaths)
        {
            if (thesePaths.Length == 1 && thesePaths[0] == null) return path;
            var startPath = path.Length > 1 ? path.TrimEnd('/', '\\') : path;
            return PathUtils.CombinePaths(new StringBuilder(startPath), thesePaths);
        }

        /// <summary>A string extension method that combine with.</summary>
        ///
        /// <param name="path">      The path to act on.</param>
        /// <param name="thesePaths">A variable-length parameters list containing these paths.</param>
        ///
        /// <returns>A string.</returns>
        public static string CombineWith(this string path, params object[] thesePaths)
        {
            if (thesePaths.Length == 1 && thesePaths[0] == null) return path;
            return PathUtils.CombinePaths(new StringBuilder(path.TrimEnd('/', '\\')), 
                thesePaths.SafeConvertAll(x => x.ToString()).ToArray());
        }

        /// <summary>A string extension method that converts a path to a parent path.</summary>
        ///
        /// <param name="path">The path to act on.</param>
        ///
        /// <returns>path as a string.</returns>
        public static string ToParentPath(this string path)
        {
            var pos = path.LastIndexOf('/');
            if (pos == -1) return "/";

            var parentPath = path.Substring(0, pos);
            return parentPath;
        }

        /// <summary>A string extension method that removes the character flags.</summary>
        ///
        /// <param name="text">     The text to act on.</param>
        /// <param name="charFlags">The character flags.</param>
        ///
        /// <returns>A string.</returns>
        public static string RemoveCharFlags(this string text, bool[] charFlags)
        {
            if (text == null) return null;

            var copy = text.ToCharArray();
            var nonWsPos = 0;

            for (var i = 0; i < text.Length; i++)
            {
                var @char = text[i];
                if (@char < charFlags.Length && charFlags[@char]) continue;
                copy[nonWsPos++] = @char;
            }

            return new String(copy, 0, nonWsPos);
        }

        /// <summary>A string extension method that converts a text to a null if empty.</summary>
        ///
        /// <param name="text">The text to act on.</param>
        ///
        /// <returns>text as a string.</returns>
        public static string ToNullIfEmpty(this string text)
        {
            return String.IsNullOrEmpty(text) ? null : text;
        }


        private static char[] SystemTypeChars = new[] { '<', '>', '+' };

        /// <summary>A Type extension method that query if 'type' is user type.</summary>
        ///
        /// <param name="type">The type to act on.</param>
        ///
        /// <returns>true if user type, false if not.</returns>
        public static bool IsUserType(this Type type)
        {
            return type.IsClass()
                && type.Namespace != null
                && !type.Namespace.StartsWith("System")
                && type.Name.IndexOfAny(SystemTypeChars) == -1;
        }

        /// <summary>A string extension method that query if 'text' is int.</summary>
        ///
        /// <param name="text">The text to act on.</param>
        ///
        /// <returns>true if int, false if not.</returns>
        public static bool IsInt(this string text)
        {
            if (String.IsNullOrEmpty(text)) return false;
            int ret;
            return Int32.TryParse(text, out ret);
        }

        /// <summary>A string extension method that converts this object to an int.</summary>
        ///
        /// <param name="text">The text to act on.</param>
        ///
        /// <returns>The given data converted to an int.</returns>
        public static int ToInt(this string text)
        {
            return text == null ? default(int) : Int32.Parse(text);
        }

        /// <summary>A string extension method that converts this object to an int.</summary>
        ///
        /// <param name="text">        The text to act on.</param>
        /// <param name="defaultValue">The default value.</param>
        ///
        /// <returns>The given data converted to an int.</returns>
        public static int ToInt(this string text, int defaultValue)
        {
            int ret;
            return Int32.TryParse(text, out ret) ? ret : defaultValue;
        }

        /// <summary>A string extension method that converts this object to an int 64.</summary>
        ///
        /// <param name="text">The text to act on.</param>
        ///
        /// <returns>The given data converted to a long.</returns>
        public static long ToInt64(this string text)
        {
            return Int64.Parse(text);
        }

        /// <summary>A string extension method that converts this object to an int 64.</summary>
        ///
        /// <param name="text">        The text to act on.</param>
        /// <param name="defaultValue">The default value.</param>
        ///
        /// <returns>The given data converted to a long.</returns>
        public static long ToInt64(this string text, long defaultValue)
        {
            long ret;
            return Int64.TryParse(text, out ret) ? ret : defaultValue;
        }

        /// <summary>A string extension method that globs.</summary>
        ///
        /// <param name="value">  The value to act on.</param>
        /// <param name="pattern">Specifies the pattern.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool Glob(this string value, string pattern)
        {
            int pos;
            for (pos = 0; pattern.Length != pos; pos++)
            {
                switch (pattern[pos])
                {
                    case '?':
                        break;

                    case '*':
                        for (int i = value.Length; i >= pos; i--)
                        {
                            if (Glob(value.Substring(i), pattern.Substring(pos + 1)))
                                return true;
                        }
                        return false;

                    default:
                        if (value.Length == pos || Char.ToUpper(pattern[pos]) != Char.ToUpper(value[pos]))
                        {
                            return false;
                        }
                        break;
                }
            }

            return value.Length == pos;
        }

    }

}