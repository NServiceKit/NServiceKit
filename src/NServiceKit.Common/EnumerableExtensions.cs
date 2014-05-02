using System;
using System.Collections.Generic;
#if WINDOWS_PHONE
using NServiceKit.Text.WP;
#endif
using NServiceKit.Common.Extensions;

namespace NServiceKit.Common
{
    /// <summary>An enumerable extensions.</summary>
    public static class EnumerableExtensions
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

        /// <summary>An IEnumerable&lt;T&gt; extension method that converts the items to a hash set.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="items">The items to act on.</param>
        ///
        /// <returns>items as a HashSet&lt;T&gt;</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            return new HashSet<T>(items);
        }

        /// <summary>An IEnumerable&lt;From&gt; extension method that safe convert all.</summary>
        ///
        /// <typeparam name="To">  Type of to.</typeparam>
        /// <typeparam name="From">Type of from.</typeparam>
        /// <param name="items">    The items to act on.</param>
        /// <param name="converter">The converter.</param>
        ///
        /// <returns>A List&lt;To&gt;</returns>
        public static List<To> SafeConvertAll<To, From>(this IEnumerable<From> items, Func<From, To> converter)
        {
            return items == null ? new List<To>() : items.ConvertAll(converter);
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that converts the items to the objects.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="items">The items to act on.</param>
        ///
        /// <returns>items as a List&lt;object&gt;</returns>
        public static List<object> ToObjects<T>(this IEnumerable<T> items)
        {
            var to = new List<object>();
            foreach (var item in items)
            {
                to.Add(item);
            }
            return to;
        }

        /// <summary>An IEnumerable&lt;string&gt; extension method that first non default or empty.</summary>
        ///
        /// <param name="values">The values to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string FirstNonDefaultOrEmpty(this IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value)) return value;
            }
            return null;
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that first non default.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="values">The values to act on.</param>
        ///
        /// <returns>A T.</returns>
        public static T FirstNonDefault<T>(this IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                if (!Equals(value, default(T))) return value;
            }
            return default(T);
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that equivalent to.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="thisList"> The thisList to act on.</param>
        /// <param name="otherList">List of others.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool EquivalentTo<T>(this IEnumerable<T> thisList, IEnumerable<T> otherList)
        {
            if (thisList == null || otherList == null) return thisList == otherList;

            var otherEnum = otherList.GetEnumerator();
            foreach (var item in thisList)
            {
                if (!otherEnum.MoveNext()) return false;

                var thisIsDefault = Equals(item, default(T));
                var otherIsDefault = Equals(otherEnum.Current, default(T));
                if (thisIsDefault || otherIsDefault)
                {
                    return thisIsDefault && otherIsDefault;
                }

                if (!item.Equals(otherEnum.Current)) return false;
            }
            var hasNoMoreLeftAsWell = !otherEnum.MoveNext();
            return hasNoMoreLeftAsWell;
        }

        /// <summary>Enumerates batches of in this collection.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="sequence"> The sequence to act on.</param>
        /// <param name="batchSize">Size of the batch.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process batches of in this collection.</returns>
        public static IEnumerable<T[]> BatchesOf<T>(this IEnumerable<T> sequence, int batchSize)
        {
            var batch = new List<T>(batchSize);
            foreach (var item in sequence)
            {
                batch.Add(item);
                if (batch.Count >= batchSize)
                {
                    yield return batch.ToArray();
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                yield return batch.ToArray();
                batch.Clear();
            }
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that converts this object to a safe dictionary.</summary>
        ///
        /// <typeparam name="T">   Generic type parameter.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="list">The list to act on.</param>
        /// <param name="expr">The expression.</param>
        ///
        /// <returns>The given data converted to a Dictionary&lt;TKey,T&gt;</returns>
        public static Dictionary<TKey, T> ToSafeDictionary<T, TKey>(this IEnumerable<T> list, Func<T, TKey> expr)
        {
            var map = new Dictionary<TKey, T>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    map[expr(item)] = item;
                }
            }
            return map;
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that converts this object to a dictionary.</summary>
        ///
        /// <typeparam name="T">     Generic type parameter.</typeparam>
        /// <typeparam name="TKey">  Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="list">The list to act on.</param>
        /// <param name="map"> The map.</param>
        ///
        /// <returns>The given data converted to a Dictionary&lt;TKey,TValue&gt;</returns>
        public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEnumerable<T> list, Func<T, KeyValuePair<TKey, TValue>> map)
        {
            var to = new Dictionary<TKey, TValue>();
            foreach (var item in list)
            {
                var entry = map(item);
                to[entry.Key] = entry.Value;
            }
            return to;
        }

    }
}