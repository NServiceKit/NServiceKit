#if !NETFX_CORE
using System;
using System.Text;

namespace NServiceKit.Logging.Support.Logging
{
    /// <summary>
    /// StringBuilderLog writes to shared StringBuffer.
    /// Made public so its testable
    /// </summary>
    public class StringBuilderLogFactory : ILogFactory
    {
        private StringBuilder sb;

        /// <summary>Initializes a new instance of the NServiceKit.Logging.Support.Logging.StringBuilderLogFactory class.</summary>
        public StringBuilderLogFactory()
        {
            sb = new StringBuilder();
        }

        /// <summary>Gets the logger.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>The logger.</returns>
        public ILog GetLogger(Type type)
        {
            return new StringBuilderLog(type, sb);
        }

        /// <summary>Gets the logger.</summary>
        ///
        /// <param name="typeName">Name of the type.</param>
        ///
        /// <returns>The logger.</returns>
        public ILog GetLogger(string typeName)
        {
            return new StringBuilderLog(typeName, sb);
        }

        /// <summary>Gets the logs.</summary>
        ///
        /// <returns>The logs.</returns>
        public string GetLogs()
        {
            lock (sb)
                return sb.ToString();
        }

        /// <summary>Clears the logs.</summary>
        public void ClearLogs()
        {
            lock (sb)
                sb.Remove(0, sb.Length - 1);
        }
    }

    /// <summary>A string builder log.</summary>
    public class StringBuilderLog : ILog
    {
        const string DEBUG = "DEBUG: ";
        const string ERROR = "ERROR: ";
        const string FATAL = "FATAL: ";
        const string INFO = "INFO: ";
        const string WARN = "WARN: ";
        private readonly StringBuilder logs;

        /// <summary>Initializes a new instance of the NServiceKit.Logging.Support.Logging.StringBuilderLog class.</summary>
        ///
        /// <param name="type">The type.</param>
        /// <param name="logs">The logs.</param>
        public StringBuilderLog(string type, StringBuilder logs)
        {
            this.logs = logs;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Logging.Support.Logging.StringBuilderLog class.</summary>
        ///
        /// <param name="type">The type.</param>
        /// <param name="logs">The logs.</param>
        public StringBuilderLog(Type type, StringBuilder logs)
        {
            this.logs = logs;
        }

        /// <summary>Gets a value indicating whether this instance is debug enabled.</summary>
        ///
        /// <value><c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.</value>
        public bool IsDebugEnabled { get { return true; } }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        private void Log(object message, Exception exception)
        {
            string msg = message == null ? string.Empty : message.ToString();
            if (exception != null)
            {
                msg += ", Exception: " + exception.Message;
            }
            lock (logs)
                logs.AppendLine(msg);
        }

        /// <summary>
        /// Logs the format.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        private void LogFormat(object message, params object[] args)
        {
            string msg = message == null ? string.Empty : message.ToString();
            lock (logs)
            {
                logs.AppendFormat(msg, args);
                logs.AppendLine();
            }
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void Log(object message)
        {
            string msg = message == null ? string.Empty : message.ToString();
            lock (logs)
            {
                logs.AppendLine(msg);
            }
        }

        /// <summary>Logs a Debug message and exception.</summary>
        ///
        /// <param name="message">  The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(object message, Exception exception)
        {
            Log(DEBUG + message, exception);
        }

        /// <summary>Logs a Debug message.</summary>
        ///
        /// <param name="message">The message.</param>
        public void Debug(object message)
        {
            Log(DEBUG + message);
        }

        /// <summary>Logs a Debug format message.</summary>
        ///
        /// <param name="format">The format.</param>
        /// <param name="args">  The args.</param>
        public void DebugFormat(string format, params object[] args)
        {
            LogFormat(DEBUG + format, args);
        }

        /// <summary>Logs a Error message and exception.</summary>
        ///
        /// <param name="message">  The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(object message, Exception exception)
        {
            Log(ERROR + message, exception);
        }

        /// <summary>Logs a Error message.</summary>
        ///
        /// <param name="message">The message.</param>
        public void Error(object message)
        {
            Log(ERROR + message);
        }

        /// <summary>Logs a Error format message.</summary>
        ///
        /// <param name="format">The format.</param>
        /// <param name="args">  The args.</param>
        public void ErrorFormat(string format, params object[] args)
        {
            LogFormat(ERROR + format, args);
        }

        /// <summary>Logs a Fatal message and exception.</summary>
        ///
        /// <param name="message">  The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(object message, Exception exception)
        {
            Log(FATAL + message, exception);
        }

        /// <summary>Logs a Fatal message.</summary>
        ///
        /// <param name="message">The message.</param>
        public void Fatal(object message)
        {
            Log(FATAL + message);
        }

        /// <summary>Logs a Error format message.</summary>
        ///
        /// <param name="format">The format.</param>
        /// <param name="args">  The args.</param>
        public void FatalFormat(string format, params object[] args)
        {
            LogFormat(FATAL + format, args);
        }

        /// <summary>Logs an Info message and exception.</summary>
        ///
        /// <param name="message">  The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(object message, Exception exception)
        {
            Log(INFO + message, exception);
        }

        /// <summary>Logs an Info message and exception.</summary>
        ///
        /// <param name="message">The message.</param>
        public void Info(object message)
        {
            Log(INFO + message);
        }

        /// <summary>Logs an Info format message.</summary>
        ///
        /// <param name="format">The format.</param>
        /// <param name="args">  The args.</param>
        public void InfoFormat(string format, params object[] args)
        {
            LogFormat(INFO + format, args);
        }

        /// <summary>Logs a Warning message and exception.</summary>
        ///
        /// <param name="message">  The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(object message, Exception exception)
        {
            Log(WARN + message, exception);
        }

        /// <summary>Logs a Warning message.</summary>
        ///
        /// <param name="message">The message.</param>
        public void Warn(object message)
        {
            Log(WARN + message);
        }

        /// <summary>Logs a Warning format message.</summary>
        ///
        /// <param name="format">The format.</param>
        /// <param name="args">  The args.</param>
        public void WarnFormat(string format, params object[] args)
        {
            LogFormat(WARN + format, args);
        }
    }
}
#endif
