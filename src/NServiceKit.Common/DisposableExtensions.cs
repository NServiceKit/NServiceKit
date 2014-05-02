using System;
using System.Collections.Generic;
using NServiceKit.Logging;

namespace NServiceKit.Common
{
    /// <summary>A disposable extensions.</summary>
    public static class DisposableExtensions
    {
        /// <summary>Releases the unmanaged resources used by the NServiceKit.Common.DisposableExtensions and optionally releases the managed resources.</summary>
        ///
        /// <param name="resources">The resources to act on.</param>
        /// <param name="log">      The log.</param>
        public static void Dispose(this IEnumerable<IDisposable> resources, ILog log)
        {
            foreach (var disposable in resources)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    if (log != null)
                    {
                        log.Error(string.Format("Error disposing of '{0}'", disposable.GetType().FullName), ex);
                    }
                }
            }
        }

        /// <summary>Releases the unmanaged resources used by the NServiceKit.Common.DisposableExtensions and optionally releases the managed resources.</summary>
        ///
        /// <param name="resources">The resources to act on.</param>
        public static void Dispose(this IEnumerable<IDisposable> resources)
        {
            Dispose(resources, null);
        }

        /// <summary>Releases the unmanaged resources used by the NServiceKit.Common.DisposableExtensions and optionally releases the managed resources.</summary>
        ///
        /// <param name="disposables">A variable-length parameters list containing disposables.</param>
        public static void Dispose(params IDisposable[] disposables)
        {
            Dispose(disposables, null);
        }

        /// <summary>A T extension method that runs.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="disposable">          The disposable to act on.</param>
        /// <param name="runActionThenDispose">The run action then dispose.</param>
        public static void Run<T>(this T disposable, Action<T> runActionThenDispose)
            where T : IDisposable
        {
            using (disposable)
            {
                runActionThenDispose(disposable);
            }
        }
    }
}