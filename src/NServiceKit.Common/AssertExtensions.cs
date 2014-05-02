using System;
using System.Collections;
using System.Collections.Generic;

namespace NServiceKit.Common
{
    /// <summary>An assert extensions.</summary>
    public static class AssertExtensions
    {
        /// <summary>Throw on first null.</summary>
        ///
        /// <param name="objs">A variable-length parameters list containing objects.</param>
        public static void ThrowOnFirstNull(params object[] objs)
        {
            foreach (var obj in objs)
            {
                ThrowIfNull(obj);
            }
        }

        /// <summary>An object extension method that throw if null.</summary>
        ///
        /// <param name="obj">The obj to act on.</param>
        public static void ThrowIfNull(this object obj)
        {
            ThrowIfNull(obj, null);
        }

        /// <summary>An object extension method that throw if null.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="obj">    The obj to act on.</param>
        /// <param name="varName">Name of the variable.</param>
        public static void ThrowIfNull(this object obj, string varName)
        {
            if (obj == null)
                throw new ArgumentNullException(varName ?? "object");
        }

        /// <summary>An ICollection extension method that throw if null or empty.</summary>
        ///
        /// <param name="strValue">The strValue to act on.</param>
        public static void ThrowIfNullOrEmpty(this string strValue)
        {
            ThrowIfNullOrEmpty(strValue, null);
        }

        /// <summary>An ICollection extension method that throw if null or empty.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="strValue">The strValue to act on.</param>
        /// <param name="varName"> Name of the variable.</param>
        public static void ThrowIfNullOrEmpty(this string strValue, string varName)
        {
            if (string.IsNullOrEmpty(strValue))
                throw new ArgumentNullException(varName ?? "string");
        }

        /// <summary>An ICollection extension method that throw if null or empty.</summary>
        ///
        /// <param name="collection">The collection to act on.</param>
        public static void ThrowIfNullOrEmpty(this ICollection collection)
        {
            ThrowIfNullOrEmpty(collection, null);
        }

        /// <summary>An ICollection extension method that throw if null or empty.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="collection">The collection to act on.</param>
        /// <param name="varName">   Name of the variable.</param>
        public static void ThrowIfNullOrEmpty(this ICollection collection, string varName)
        {
            var fieldName = varName ?? "collection";

            if (collection == null)
                throw new ArgumentNullException(fieldName);

            if (collection.Count == 0)
                throw new ArgumentException(fieldName + " is empty");
        }

        /// <summary>An ICollection&lt;T&gt; extension method that throw if null or empty.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        public static void ThrowIfNullOrEmpty<T>(this ICollection<T> collection)
        {
            ThrowIfNullOrEmpty(collection, null);
        }

        /// <summary>An ICollection&lt;T&gt; extension method that throw if null or empty.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="varName">   Name of the variable.</param>
        public static void ThrowIfNullOrEmpty<T>(this ICollection<T> collection, string varName)
        {
            var fieldName = varName ?? "collection";

            if (collection == null)
                throw new ArgumentNullException(fieldName);

            if (collection.Count == 0)
                throw new ArgumentException(fieldName + " is empty");
        }

    }

}