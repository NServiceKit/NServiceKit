using System;
using System.Collections;
using System.Collections.Generic;
using Proxy = NServiceKit.Common;

namespace NServiceKit.Common.Extensions
{
    /// <summary>A collection extensions.</summary>
    [Obsolete("Use NServiceKit.Common.ByteArrayExtensions")]
    public static class CollectionExtensions
    {
        /// <summary>An ICollection&lt;T&gt; extension method that query if 'collection' is empty.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        ///
        /// <returns>true if empty, false if not.</returns>
        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>An ICollection extension method that convert all.</summary>
        ///
        /// <typeparam name="To">Type of to.</typeparam>
        /// <param name="items">    The items to act on.</param>
        /// <param name="converter">The converter.</param>
        ///
        /// <returns>all converted.</returns>
        public static List<To> ConvertAll<To>(this ICollection items, Func<object, To> converter)
        {
            var list = new List<To>();
            foreach (var item in items)
            {
                list.Add(converter(item));
            }
            return list;
        }
    }
}