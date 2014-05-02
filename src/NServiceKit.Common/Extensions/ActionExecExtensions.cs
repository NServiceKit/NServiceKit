using System;
using System.Collections.Generic;
using System.Threading;

using Proxy = NServiceKit.Common.ActionExecExtensions;

namespace NServiceKit.Common.Extensions
{
    /// <summary>The extensions proxy.</summary>
    [Obsolete("Use NServiceKit.Common.ActionExecExtensions")]
    public static class ExtensionsProxy
    {
        /// <summary>An ICollection&lt;Action&gt; extension method that executes all and wait operation.</summary>
        ///
        /// <param name="actions">The actions to act on.</param>
        /// <param name="timeout">The timeout.</param>
        public static void ExecAllAndWait(this ICollection<Action> actions, TimeSpan timeout)
        {
            Proxy.ExecAllAndWait(actions, timeout);
        }

        /// <summary>An IEnumerable&lt;Action&gt; extension method that executes the asynchronous operation.</summary>
        ///
        /// <param name="actions">The actions to act on.</param>
        ///
        /// <returns>A List&lt;WaitHandle&gt;</returns>
        public static List<WaitHandle> ExecAsync(this IEnumerable<Action> actions)
        {
            return Proxy.ExecAsync(actions);
        }

        /// <summary>Wait all.</summary>
        ///
        /// <param name="waitHandles">The wait handles.</param>
        /// <param name="timeoutMs">  The timeout in milliseconds.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(this List<WaitHandle> waitHandles, int timeoutMs)
        {
            return Proxy.WaitAll(waitHandles, timeoutMs);
        }

        /// <summary>Wait all.</summary>
        ///
        /// <param name="waitHandles">The wait handles.</param>
        /// <param name="timeoutMs">  The timeout in milliseconds.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(this ICollection<WaitHandle> waitHandles, int timeoutMs)
        {
            return Proxy.WaitAll(waitHandles, timeoutMs);
        }

        /// <summary>Wait all.</summary>
        ///
        /// <param name="waitHandles">The wait handles.</param>
        /// <param name="timeout">    The timeout.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(this ICollection<WaitHandle> waitHandles, TimeSpan timeout)
        {
            return Proxy.WaitAll(waitHandles, timeout);
        }

#if !SILVERLIGHT && !MONOTOUCH && !XBOX

        /// <summary>Wait all.</summary>
        ///
        /// <param name="asyncResults">The asyncResults to act on.</param>
        /// <param name="timeout">     The timeout.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(this List<IAsyncResult> asyncResults, TimeSpan timeout)
        {
            return Proxy.WaitAll(asyncResults, timeout);
        }
#endif

        /// <summary>Wait all.</summary>
        ///
        /// <param name="waitHandles">The wait handles.</param>
        /// <param name="timeout">    The timeout.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(WaitHandle[] waitHandles, TimeSpan timeout)
        {
            return Proxy.WaitAll(waitHandles, timeout);
        }

        /// <summary>Wait all.</summary>
        ///
        /// <param name="waitHandles">The wait handles.</param>
        /// <param name="timeOutMs">  The time out in milliseconds.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(WaitHandle[] waitHandles, int timeOutMs)
        {
            return Proxy.WaitAll(waitHandles, timeOutMs);
        }	
    }
}