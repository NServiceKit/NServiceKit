#if !SILVERLIGHT && !MONOTOUCH && !XBOX

using System;
using System.Diagnostics;

namespace NServiceKit.Common.Utils
{
    /// <summary>A performance utilities.</summary>
    public static class PerfUtils
    {
        /// <summary>A long extension method that converts the fromTicks to a time span.</summary>
        ///
        /// <param name="fromTicks">The fromTicks to act on.</param>
        ///
        /// <returns>fromTicks as a TimeSpan.</returns>
        public static TimeSpan ToTimeSpan(this long fromTicks)
        {
            return TimeSpan.FromSeconds(fromTicks * 1d / Stopwatch.Frequency);
        }

        /// <summary>Measures.</summary>
        ///
        /// <param name="iterations">The iterations.</param>
        /// <param name="action">    The action.</param>
        ///
        /// <returns>A long.</returns>
        public static long Measure(long iterations, Action action)
        {
            GC.Collect();
            var begin = Stopwatch.GetTimestamp();

            for (var i = 0; i < iterations; i++)
            {
                action();
            }

            var end = Stopwatch.GetTimestamp();

            return (end - begin);
        }
    }
}

#endif