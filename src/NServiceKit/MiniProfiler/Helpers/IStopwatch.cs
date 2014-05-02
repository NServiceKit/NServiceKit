using System.Diagnostics;

namespace NServiceKit.MiniProfiler.Helpers
{

    internal interface IStopwatch
    {
        /// <summary>Gets the elapsed ticks.</summary>
        ///
        /// <value>The elapsed ticks.</value>
        long ElapsedTicks { get; }

        /// <summary>Gets the frequency.</summary>
        ///
        /// <value>The frequency.</value>
        long Frequency { get; }

        /// <summary>Gets a value indicating whether this object is running.</summary>
        ///
        /// <value>true if this object is running, false if not.</value>
        bool IsRunning { get; }
        /// <summary>Stops this object.</summary>
        void Stop();
    }

    internal class StopwatchWrapper : IStopwatch
    {
        /// <summary>Starts a new.</summary>
        ///
        /// <returns>An IStopwatch.</returns>
        public static IStopwatch StartNew()
        {
            return new StopwatchWrapper();
        }

        private Stopwatch _sw;

        private StopwatchWrapper()
        {
            _sw = Stopwatch.StartNew();
        }

        /// <summary>Gets the elapsed ticks.</summary>
        ///
        /// <value>The elapsed ticks.</value>
        public long ElapsedTicks
        {
            get { return _sw.ElapsedTicks; }
        }

        /// <summary>Gets the frequency.</summary>
        ///
        /// <value>The frequency.</value>
        public long Frequency
        {
            get { return Stopwatch.Frequency; }
        }

        /// <summary>Gets a value indicating whether this object is running.</summary>
        ///
        /// <value>true if this object is running, false if not.</value>
        public bool IsRunning
        {
            get { return _sw.IsRunning; }
        }

        /// <summary>Stops this object.</summary>
        public void Stop()
        {
            _sw.Stop();
        }
    }

}
