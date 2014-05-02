using System;
using System.Collections.Generic;

using Proxy = NServiceKit.Common.IntExtensions;

namespace NServiceKit.Common.Extensions
{
    /// <summary>An int extensions.</summary>
    [Obsolete("Use NServiceKit.Common.IntExtensions")]
    public static class IntExtensions
    {
        /// <summary>An int extension method that times.</summary>
        ///
        /// <param name="times">The times to act on.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process times in this collection.</returns>
        public static IEnumerable<int> Times(this int times)
        {
            return Proxy.Times(times);
        }

        /// <summary>An int extension method that times.</summary>
        ///
        /// <param name="times">   The times to act on.</param>
        /// <param name="actionFn">The action function.</param>
        public static void Times(this int times, Action<int> actionFn)
        {
            Proxy.Times(times, actionFn);
        }

        /// <summary>An int extension method that times.</summary>
        ///
        /// <param name="times">   The times to act on.</param>
        /// <param name="actionFn">The action function.</param>
        public static void Times(this int times, Action actionFn)
        {
            Proxy.Times(times, actionFn);
        }

        /// <summary>An int extension method that times asynchronous.</summary>
        ///
        /// <param name="times">   The times to act on.</param>
        /// <param name="actionFn">The action function.</param>
        ///
        /// <returns>A List&lt;IAsyncResult&gt;</returns>
        public static List<IAsyncResult> TimesAsync(this int times, Action<int> actionFn)
        {
            return Proxy.TimesAsync(times, actionFn);
        }

        /// <summary>An int extension method that times asynchronous.</summary>
        ///
        /// <param name="times">   The times to act on.</param>
        /// <param name="actionFn">The action function.</param>
        ///
        /// <returns>A List&lt;IAsyncResult&gt;</returns>
        public static List<IAsyncResult> TimesAsync(this int times, Action actionFn)
        {
            return Proxy.TimesAsync(times, actionFn);
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
            return Proxy.Times(times, actionFn);
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
            return Proxy.Times(times, actionFn);
        }
    }
}