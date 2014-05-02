using System;
using System.Collections;
using System.Collections.Generic;

using Proxy = NServiceKit.Common.AssertExtensions;

namespace NServiceKit.Common.Extensions
{
    /// <summary>An assert extensions.</summary>
    [Obsolete("Use NServiceKit.Common.AssertExtensions")]
    public static class AssertExtensions
    {
        /// <summary>Throw on first null.</summary>
        ///
        /// <param name="objs">A variable-length parameters list containing objects.</param>
        public static void ThrowOnFirstNull(params object[] objs)
        {
            Proxy.ThrowOnFirstNull(objs);
        }

        /// <summary>An object extension method that throw if null.</summary>
        ///
        /// <param name="obj">The obj to act on.</param>
        public static void ThrowIfNull(this object obj)
        {
            Proxy.ThrowIfNull(obj);
        }

        /// <summary>An object extension method that throw if null.</summary>
        ///
        /// <param name="obj">    The obj to act on.</param>
        /// <param name="varName">Name of the variable.</param>
        public static void ThrowIfNull(this object obj, string varName)
        {
            Proxy.ThrowIfNull(obj, varName);
        }

        /// <summary>An ICollection extension method that throw if null or empty.</summary>
        ///
        /// <param name="strValue">The strValue to act on.</param>
        public static void ThrowIfNullOrEmpty(this string strValue)
        {
            Proxy.ThrowIfNullOrEmpty(strValue);
        }

        /// <summary>An ICollection extension method that throw if null or empty.</summary>
        ///
        /// <param name="strValue">The strValue to act on.</param>
        /// <param name="varName"> Name of the variable.</param>
        public static void ThrowIfNullOrEmpty(this string strValue, string varName)
        {
            Proxy.ThrowIfNullOrEmpty(strValue, varName);
        }

        /// <summary>An ICollection extension method that throw if null or empty.</summary>
        ///
        /// <param name="collection">The collection to act on.</param>
        public static void ThrowIfNullOrEmpty(this ICollection collection)
        {
            Proxy.ThrowIfNullOrEmpty(collection);
        }

        /// <summary>An ICollection extension method that throw if null or empty.</summary>
        ///
        /// <param name="collection">The collection to act on.</param>
        /// <param name="varName">   Name of the variable.</param>
        public static void ThrowIfNullOrEmpty(this ICollection collection, string varName)
        {
            Proxy.ThrowIfNullOrEmpty(collection, varName);
        }

        /// <summary>An ICollection&lt;T&gt; extension method that throw if null or empty.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        public static void ThrowIfNullOrEmpty<T>(this ICollection<T> collection)
        {
            Proxy.ThrowIfNullOrEmpty(collection);
        }

        /// <summary>An ICollection&lt;T&gt; extension method that throw if null or empty.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="varName">   Name of the variable.</param>
        public static void ThrowIfNullOrEmpty<T>(this ICollection<T> collection, string varName)
        {
            Proxy.ThrowIfNullOrEmpty(collection, varName);
        }
         
    }
}