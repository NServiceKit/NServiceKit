using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NServiceKit.Logging;

namespace NServiceKit.Common
{
    /// <summary>An execute extensions.</summary>
    public static class ExecExtensions
    {
        /// <summary>Logs an error.</summary>
        ///
        /// <param name="declaringType">   Type of the declaring.</param>
        /// <param name="clientMethodName">Name of the client method.</param>
        /// <param name="ex">              The ex.</param>
        public static void LogError(Type declaringType, string clientMethodName, Exception ex)
        {
            var log = LogManager.GetLogger(declaringType);
            log.Error(string.Format("'{0}' threw an error on {1}: {2}", declaringType.FullName, clientMethodName, ex.Message), ex);
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that executes all operation.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instances">The instances to act on.</param>
        /// <param name="action">   The action.</param>
        public static void ExecAll<T>(this IEnumerable<T> instances, Action<T> action)
        {
            foreach (var instance in instances)
            {
                try
                {
                    action(instance);
                }
                catch (Exception ex)
                {
                    LogError(instance.GetType(), action.GetType().Name, ex);
                }
            }
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that executes all with first out operation.</summary>
        ///
        /// <typeparam name="T">      Generic type parameter.</typeparam>
        /// <typeparam name="TReturn">Type of the return.</typeparam>
        /// <param name="instances">  The instances to act on.</param>
        /// <param name="action">     The action.</param>
        /// <param name="firstResult">The first result.</param>
        public static void ExecAllWithFirstOut<T, TReturn>(this IEnumerable<T> instances, Func<T, TReturn> action, ref TReturn firstResult)
        {
            foreach (var instance in instances)
            {
                try
                {
                    var result = action(instance);
                    if (!Equals(firstResult, default(TReturn)))
                    {
                        firstResult = result;
                    }
                }
                catch (Exception ex)
                {
                    LogError(instance.GetType(), action.GetType().Name, ex);
                }
            }
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that executes the return first with result operation.</summary>
        ///
        /// <typeparam name="T">      Generic type parameter.</typeparam>
        /// <typeparam name="TReturn">Type of the return.</typeparam>
        /// <param name="instances">The instances to act on.</param>
        /// <param name="action">   The action.</param>
        ///
        /// <returns>A TReturn.</returns>
        public static TReturn ExecReturnFirstWithResult<T, TReturn>(this IEnumerable<T> instances, Func<T, TReturn> action)
        {
            foreach (var instance in instances)
            {
                try
                {
                    var result = action(instance);
                    if (!Equals(result, default(TReturn)))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    LogError(instance.GetType(), action.GetType().Name, ex);
                }
            }

            return default(TReturn);
        }

        /// <summary>Retry until true.</summary>
        ///
        /// <exception cref="TimeoutException">Thrown when a Timeout error condition occurs.</exception>
        ///
        /// <param name="action"> The action.</param>
        /// <param name="timeOut">The time out.</param>
        public static void RetryUntilTrue(Func<bool> action, TimeSpan? timeOut)
        {
            var i = 0;
            var firstAttempt = DateTime.UtcNow;

            while (timeOut == null || DateTime.UtcNow - firstAttempt < timeOut.Value)
            {
                i++;
                if (action())
                {
                    return;
                }
                SleepBackOffMultiplier(i);
            }

            throw new TimeoutException(string.Format("Exceeded timeout of {0}", timeOut.Value));
        }

        /// <summary>Retry on exception.</summary>
        ///
        /// <exception cref="TimeoutException">Thrown when a Timeout error condition occurs.</exception>
        ///
        /// <param name="action"> The action.</param>
        /// <param name="timeOut">The time out.</param>
        public static void RetryOnException(Action action, TimeSpan? timeOut)
        {
            var i = 0;
            Exception lastEx = null;
            var firstAttempt = DateTime.UtcNow;

            while (timeOut == null || DateTime.UtcNow - firstAttempt < timeOut.Value)
            {
                i++;
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    lastEx = ex;

                    SleepBackOffMultiplier(i);
                }
            }

            throw new TimeoutException(string.Format("Exceeded timeout of {0}", timeOut.Value), lastEx);
        }

        /// <summary>Retry on exception.</summary>
        ///
        /// <param name="action">    The action.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        public static void RetryOnException(Action action, int maxRetries)
        {
            for (var i = 0; i < maxRetries; i++)
            {
                try
                {
                    action();
                    break;
                }
                catch
                {
                    if (i == maxRetries - 1) throw;

                    SleepBackOffMultiplier(i);
                }
            }
        }

        private static void SleepBackOffMultiplier(int i)
        {
            //exponential/random retry back-off.
            var rand = new Random(Guid.NewGuid().GetHashCode());
            var nextTry = rand.Next(
                (int)Math.Pow(i, 2), (int)Math.Pow(i + 1, 2) + 1);

            Thread.Sleep(nextTry);
        }
    }
}
