using System;
using System.Collections.Generic;

using Proxy = NServiceKit.Common.DictionaryExtensions;

namespace NServiceKit.Common.Extensions
{
    /// <summary>A dictionary extensions.</summary>
    [Obsolete("Use NServiceKit.Common.DictionaryExtensions")]
    public static class DictionaryExtensions
    {
        /// <summary>A Dictionary&lt;TKey,TValue&gt; extension method that gets value or default.</summary>
        ///
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <typeparam name="TKey">  Type of the key.</typeparam>
        /// <param name="dictionary">The dictionary to act on.</param>
        /// <param name="key">       The key.</param>
        ///
        /// <returns>The value or default.</returns>
        public static TValue GetValueOrDefault<TValue, TKey>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return Proxy.GetValueOrDefault(dictionary, key);
        }

        /// <summary>A Dictionary&lt;TKey,TValue&gt; extension method that applies an operation to all items in this collection.</summary>
        ///
        /// <typeparam name="TKey">  Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to act on.</param>
        /// <param name="onEachFn">  The on each function.</param>
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Action<TKey, TValue> onEachFn)
        {
            Proxy.ForEach(dictionary, onEachFn);
        }

        /// <summary>An IDictionary&lt;K,V&gt; extension method that equivalent to.</summary>
        ///
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="thisMap"> The thisMap to act on.</param>
        /// <param name="otherMap">The other map.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool EquivalentTo<K, V>(this IDictionary<K, V> thisMap, IDictionary<K, V> otherMap)
        {
            return Proxy.EquivalentTo(thisMap, otherMap);
        }

        /// <summary>Convert all.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="map">     The map.</param>
        /// <param name="createFn">The create function.</param>
        ///
        /// <returns>all converted.</returns>
        public static List<T> ConvertAll<T, K, V>(IDictionary<K, V> map, Func<K, V, T> createFn)
        {
            return Proxy.ConvertAll(map, createFn);
        }
    }
}