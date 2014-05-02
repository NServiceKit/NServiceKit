using System;
using System.Collections.Generic;

namespace NServiceKit.Common
{
    /// <summary>An int extensions.</summary>
    public static class IntExtensions
    {
        /// <summary>An int extension method that times.</summary>
        ///
        /// <param name="times">The times to act on.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process times in this collection.</returns>
        public static IEnumerable<int> Times(this int times)
        {
            for (var i=0; i < times; i++)
            {
                yield return i;
            }
        }

        /// <summary>An int extension method that times.</summary>
        ///
        /// <param name="times">   The times to act on.</param>
        /// <param name="actionFn">The action function.</param>
        public static void Times(this int times, Action<int> actionFn)
        {
            for (var i = 0; i < times; i++)
            {
                actionFn(i);
            }
        }

        /// <summary>An int extension method that times.</summary>
        ///
        /// <param name="times">   The times to act on.</param>
        /// <param name="actionFn">The action function.</param>
        public static void Times(this int times, Action actionFn)
        {
            for (var i = 0; i < times; i++)
            {
                actionFn();
            }
        }

        /// <summary>An int extension method that times asynchronous.</summary>
        ///
        /// <param name="times">   The times to act on.</param>
        /// <param name="actionFn">The action function.</param>
        ///
        /// <returns>A List&lt;IAsyncResult&gt;</returns>
        public static List<IAsyncResult> TimesAsync(this int times, Action<int> actionFn)
        {
            var asyncResults = new List<IAsyncResult>(times);
            for (var i = 0; i < times; i++)
            {
                asyncResults.Add(actionFn.BeginInvoke(i, null, null));				
            }
            return asyncResults;
        }

        /// <summary>An int extension method that times asynchronous.</summary>
        ///
        /// <param name="times">   The times to act on.</param>
        /// <param name="actionFn">The action function.</param>
        ///
        /// <returns>A List&lt;IAsyncResult&gt;</returns>
        public static List<IAsyncResult> TimesAsync(this int times, Action actionFn)
        {
            var asyncResults = new List<IAsyncResult>(times);
            for (var i = 0; i < times; i++)
            {
                asyncResults.Add(actionFn.BeginInvoke(null, null));
            }
            return asyncResults;
        }

        /// <summary>An int extension method that times.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="times">   The times to act on.</param>
        /// <param name="actionFn">The action function.</param>
        ///
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Times<T>(this int times, Func<T> actionFn)
        {
            var list = new List<T>();
            for (var i=0; i < times; i++)
            {
                list.Add(actionFn());
            }
            return list;
        }

        /// <summary>An int extension method that times.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="times">   The times to act on.</param>
        /// <param name="actionFn">The action function.</param>
        ///
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Times<T>(this int times, Func<int, T> actionFn)
        {
            var list = new List<T>();
            for (var i=0; i < times; i++)
            {
                list.Add(actionFn(i));
            }
            return list;
        }
    }
}