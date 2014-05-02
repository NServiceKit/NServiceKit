using System;
using NServiceKit.Logging;

namespace NServiceKit.Common.Utils
{
    /// <summary>A function utilities.</summary>
    public static class FuncUtils
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FuncUtils));

        /// <summary>
        /// Invokes the action provided and returns true if no excpetion was thrown.
        /// Otherwise logs the exception and returns false if an exception was thrown.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static bool TryExec(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            return false;
        }

        /// <summary>Try execute.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="func">The function.</param>
        ///
        /// <returns>A T.</returns>
        public static T TryExec<T>(Func<T> func)
        {
            return TryExec(func, default(T));
        }

        /// <summary>Try execute.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="func">        The function.</param>
        /// <param name="defaultValue">The default value.</param>
        ///
        /// <returns>A T.</returns>
        public static T TryExec<T>(Func<T> func, T defaultValue)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            return default(T);
        }

#if !SILVERLIGHT //No Stopwatch

        /// <summary>Wait while.</summary>
        ///
        /// <exception cref="TimeoutException">Thrown when a Timeout error condition occurs.</exception>
        ///
        /// <param name="condition">           The condition.</param>
        /// <param name="millisecondTimeout">  The millisecond timeout.</param>
        /// <param name="millsecondPollPeriod">The millsecond poll period.</param>
        public static void WaitWhile(Func<bool> condition, int millisecondTimeout, int millsecondPollPeriod = 10)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            while (condition())
            {
                System.Threading.Thread.Sleep(millsecondPollPeriod);
                if (timer.ElapsedMilliseconds > millisecondTimeout)
                    throw new TimeoutException("Timed out waiting for condition function.");
            }
        }
#endif

    }

}