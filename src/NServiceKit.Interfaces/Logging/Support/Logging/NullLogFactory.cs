using System;

namespace NServiceKit.Logging.Support.Logging
{
    /// <summary>
    /// Creates a Debug Logger, that logs all messages to: System.Diagnostics.Debug
    /// 
    /// Made public so its testable
    /// </summary>
	public class NullLogFactory : ILogFactory
    {
        /// <summary>Gets the logger.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>The logger.</returns>
        public ILog GetLogger(Type type)
        {
			return new NullDebugLogger(type);
        }

        /// <summary>Gets the logger.</summary>
        ///
        /// <param name="typeName">Name of the type.</param>
        ///
        /// <returns>The logger.</returns>
        public ILog GetLogger(string typeName)
        {
			return new NullDebugLogger(typeName);
        }
    }
}
