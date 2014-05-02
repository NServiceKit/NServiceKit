using System;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.Common.Extensions;

namespace NServiceKit.Common
{
    /// <summary>A dictionary extensions.</summary>
    public static class DictionaryExtensions
    {
        /// <summary>A Dictionary&lt;TKey,TValue&gt; extension method that gets value or default.</summary>
        ///
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <typeparam name="TKey">  Type of the key.</typeparam>
        /// <param name="dictionary">The dictionary to act on.</param>
        /// <param name="key">       The key to act on.</param>
        ///
        /// <returns>The value or default.</returns>
        public static TValue GetValueOrDefault<TValue, TKey>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : default(TValue);
        }

        /// <summary>A Dictionary&lt;TKey,TValue&gt; extension method that applies an operation to all items in this collection.</summary>
        ///
        /// <typeparam name="TKey">  Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to act on.</param>
        /// <param name="onEachFn">  The on each function.</param>
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Action<TKey, TValue> onEachFn)
        {
            foreach (var entry in dictionary)
            {
                onEachFn(entry.Key, entry.Value);
            }
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
            if (thisMap == null || otherMap == null) return thisMap == otherMap;
            if (thisMap.Count != otherMap.Count) return false;

            foreach (var entry in thisMap)
            {
                V otherValue;
                if (!otherMap.TryGetValue(entry.Key, out otherValue)) return false;
                if (!Equals(entry.Value, otherValue)) return false;
            }

            return true;
        }

        /// <summary>Convert all.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="map">     The map to act on.</param>
        /// <param name="createFn">The create function.</param>
        ///
        /// <returns>all converted.</returns>
        public static List<T> ConvertAll<T, K, V>(IDictionary<K, V> map, Func<K, V, T> createFn)
        {
            var list = new List<T>();
            map.ForEach((kvp) => list.Add(createFn(kvp.Key, kvp.Value)));
            return list;
        }

        /// <summary>A Dictionary&lt;K,V&gt; extension method that gets or add.</summary>
        ///
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="map">     The map to act on.</param>
        /// <param name="key">     The key to act on.</param>
        /// <param name="createFn">The create function.</param>
        ///
        /// <returns>The or add.</returns>
        public static V GetOrAdd<K, V>(this Dictionary<K, V> map, K key, Func<K, V> createFn)
        {
            //simulate ConcurrentDictionary.GetOrAdd
            lock (map)
            {
                V val;
                if (!map.TryGetValue(key, out val))
                    map[key] = val = createFn(key);

                return val;
            }
        }

        /// <summary>A TKey extension method that pair with.</summary>
        ///
        /// <typeparam name="TKey">  Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="key">  The key to act on.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A KeyValuePair&lt;TKey,TValue&gt;</returns>
        public static KeyValuePair<TKey, TValue> PairWith<TKey, TValue>(this TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}