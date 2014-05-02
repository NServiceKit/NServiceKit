using System;

namespace NServiceKit.Logging.Support.Logging
{
    /// <summary>
    /// Creates a test Logger, that stores all log messages in a member list
    /// </summary>
	public class TestLogFactory : ILogFactory
    {
        /// <summary>Gets the logger.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>The logger.</returns>
        public ILog GetLogger(Type type)
        {
            return new TestLogger(type);
        }

        /// <summary>Gets the logger.</summary>
        ///
        /// <param name="typeName">Name of the type.</param>
        ///
        /// <returns>The logger.</returns>
        public ILog GetLogger(string typeName)
        {
            return new TestLogger(typeName);
        }
    }
}
