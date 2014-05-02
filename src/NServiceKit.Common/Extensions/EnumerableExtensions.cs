using System;
using System.Collections;
using System.Collections.Generic;
#if WINDOWS_PHONE
using NServiceKit.Text.WP;
#endif

using Proxy = NServiceKit.Common.EnumerableExtensions;

namespace NServiceKit.Common.Extensions
{
    /// <summary>
    /// These extensions have a potential to conflict with the LINQ extensions methods so
    /// leaving the implmentation in the 'Extensions' sub-namespace to force explicit opt-in
    /// </summary>
    [Obsolete("Use NServiceKit.Common.EnumerableExtensions")]
    public static class EnumerableExtensions
    {
        /// <summary>An IEnumerable&lt;T&gt; extension method that applies an operation to all items in this collection.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="values">The values to act on.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
            {
                action(value);
            }
        }

        /// <summary>An IEnumerable extension method that convert all.</summary>
        ///
        /// <typeparam name="To">Type of to.</typeparam>
        /// <param name="items">    The items to act on.</param>
        /// <param name="converter">The converter.</param>
        ///
        /// <returns>all converted.</returns>
        public static List<To> ConvertAll<To>(this IEnumerable items, Func<object, To> converter)
        {
            var list = new List<To>();
            foreach (var item in items)
            {
                list.Add(converter(item));
            }
            return list;
        }

        /// <summary>An IEnumerable extension method that firsts the given items.</summary>
        ///
        /// <param name="items">The items to act on.</param>
        ///
        /// <returns>An object.</returns>
        public static object First(this IEnumerable items)
        {
            foreach (var item in items)
            {
                return item;
            }
            return null;
        }

        /// <summary>An IEnumerable extension method that converts the items to a list.</summary>
        ///
        /// <typeparam name="To">Type of to.</typeparam>
        /// <param name="items">The items to act on.</param>
        ///
        /// <returns>items as a List&lt;To&gt;</returns>
        public static List<To> ToList<To>(this IEnumerable items)
        {
            var list = new List<To>();
            foreach (var item in items)
            {
                list.Add((To)item);
            }
            return list;
        }

        /// <summary>An IEnumerable&lt;From&gt; extension method that convert all.</summary>
        ///
        /// <typeparam name="To">  Type of to.</typeparam>
        /// <typeparam name="From">Type of from.</typeparam>
        /// <param name="items">    The items to act on.</param>
        /// <param name="converter">The converter.</param>
        ///
        /// <returns>all converted.</returns>
        public static List<To> ConvertAll<To, From>(this IEnumerable<From> items, Func<From, To> converter)
        {
            var list = new List<To>();
            foreach (var item in items)
            {
                list.Add(converter(item));
            }
            return list;
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that converts the items to a hash set.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="items">The items to act on.</param>
        ///
        /// <returns>items as a HashSet&lt;T&gt;</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            return Proxy.ToHashSet(items);
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
            return Proxy.SafeConvertAll(items, converter);
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

    }
}