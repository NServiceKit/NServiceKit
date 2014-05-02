using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NServiceKit.Common.Support;

#if NETFX_CORE
using Windows.System.Threading;
#endif

namespace NServiceKit.Common
{
    /// <summary>An action execute extensions.</summary>
    public static class ActionExecExtensions
    {
        /// <summary>An ICollection&lt;Action&gt; extension method that executes all and wait operation.</summary>
        ///
        /// <param name="actions">The actions to act on.</param>
        /// <param name="timeout">The timeout.</param>
        public static void ExecAllAndWait(this ICollection<Action> actions, TimeSpan timeout)
        {
            var waitHandles = new WaitHandle[actions.Count];
            var i = 0;
            foreach (var action in actions)
            {
                waitHandles[i++] = action.BeginInvoke(null, null).AsyncWaitHandle;
            }

            WaitAll(waitHandles, timeout);
        }

        /// <summary>An IEnumerable&lt;Action&gt; extension method that executes the asynchronous operation.</summary>
        ///
        /// <param name="actions">The actions to act on.</param>
        ///
        /// <returns>A List&lt;WaitHandle&gt;</returns>
        public static List<WaitHandle> ExecAsync(this IEnumerable<Action> actions)
        {
            var waitHandles = new List<WaitHandle>();
            foreach (var action in actions)
            {
                var waitHandle = new AutoResetEvent(false);
                waitHandles.Add(waitHandle);
                var commandExecsHandler = new ActionExecHandler(action, waitHandle);
#if NETFX_CORE
                ThreadPool.RunAsync(new WorkItemHandler((IAsyncAction) => commandExecsHandler.Execute()));
#else
                ThreadPool.QueueUserWorkItem(x => ((ActionExecHandler)x).Execute(), commandExecsHandler);
#endif
            }
            return waitHandles;
        }

        /// <summary>Wait all.</summary>
        ///
        /// <param name="waitHandles">The wait handles.</param>
        /// <param name="timeoutMs">  The timeout in milliseconds.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(this List<WaitHandle> waitHandles, int timeoutMs)
        {
            return WaitAll(waitHandles.ToArray(), timeoutMs);
        }

        /// <summary>Wait all.</summary>
        ///
        /// <param name="waitHandles">The wait handles.</param>
        /// <param name="timeoutMs">  The timeout in milliseconds.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(this ICollection<WaitHandle> waitHandles, int timeoutMs)
        {
            return WaitAll(waitHandles.ToArray(), timeoutMs);
        }

        /// <summary>Wait all.</summary>
        ///
        /// <param name="waitHandles">The wait handles.</param>
        /// <param name="timeout">    The timeout.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(this ICollection<WaitHandle> waitHandles, TimeSpan timeout)
        {
            return WaitAll(waitHandles.ToArray(), (int)timeout.TotalMilliseconds);
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
            var waitHandles = asyncResults.ConvertAll(x => x.AsyncWaitHandle);
            return WaitAll(waitHandles.ToArray(), (int)timeout.TotalMilliseconds);
        }

        /// <summary>Wait all.</summary>
        ///
        /// <param name="waitHandles">The wait handles.</param>
        /// <param name="timeout">    The timeout.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(WaitHandle[] waitHandles, TimeSpan timeout)
        {
            return WaitAll(waitHandles, (int)timeout.TotalMilliseconds);
        }

        /// <summary>Wait all.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="waitHandles">The wait handles.</param>
        /// <param name="timeOutMs">  The time out in milliseconds.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WaitAll(WaitHandle[] waitHandles, int timeOutMs)
        {
            // throws an exception if there are no wait handles
            if (waitHandles == null) throw new ArgumentNullException("waitHandles");
            if (waitHandles.Length == 0) return true;

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                // WaitAll for multiple handles on an STA thread is not supported.
                // CurrentThread is ApartmentState.STA when run under unit tests
                var successfullyComplete = true;
                foreach (var waitHandle in waitHandles)
                {
                    successfullyComplete = successfullyComplete 
                        && waitHandle.WaitOne(timeOutMs, false);
                }
                return successfullyComplete;
            }

            return WaitHandle.WaitAll(waitHandles, timeOutMs, false);
        }
#endif

    }

}