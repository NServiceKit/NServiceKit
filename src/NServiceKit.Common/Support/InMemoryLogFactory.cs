using System;
using System.Collections.Generic;
using System.Text;
using NServiceKit.Logging;

namespace NServiceKit.Common.Support
{
    /// <summary>
    /// Note: InMemoryLog keeps all logs in memory, so don't use it long running exceptions
    /// 
    /// Returns a thread-safe InMemoryLog which you can use while *TESTING*
    /// to provide a detailed analysis of your logs.
    /// </summary>
    public class InMemoryLogFactory
        : ILogFactory
    {
        /// <summary>Gets the logger.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>The logger.</returns>
        public ILog GetLogger(Type type)
        {
            return new InMemoryLog(type.Name);
        }

        /// <summary>Gets the logger.</summary>
        ///
        /// <param name="typeName">Name of the type.</param>
        ///
        /// <returns>The logger.</returns>
        public ILog GetLogger(string typeName)
        {
            return new InMemoryLog(typeName);
        }
    }

    /// <summary>An in memory log.</summary>
    public class InMemoryLog
        : ILog
    {
        private readonly object syncLock = new object();

        /// <summary>Gets the name of the logger.</summary>
        ///
        /// <value>The name of the logger.</value>
        public string LoggerName { get; private set; }

        /// <summary>Gets the combined log.</summary>
        ///
        /// <value>The combined log.</value>
        public StringBuilder CombinedLog { get; private set; }

        /// <summary>Gets or sets the debug entries.</summary>
        ///
        /// <value>The debug entries.</value>
        public List<string> DebugEntries { get; set; }

        /// <summary>Gets or sets the debug exceptions.</summary>
        ///
        /// <value>The debug exceptions.</value>
        public List<Exception> DebugExceptions { get; set; }

        /// <summary>Gets or sets the information entries.</summary>
        ///
        /// <value>The information entries.</value>
        public List<string> InfoEntries { get; set; }

        /// <summary>Gets or sets the information exceptions.</summary>
        ///
        /// <value>The information exceptions.</value>
        public List<Exception> InfoExceptions { get; set; }

        /// <summary>Gets or sets the warning entries.</summary>
        ///
        /// <value>The warning entries.</value>
        public List<string> WarnEntries { get; set; }

        /// <summary>Gets or sets the warning exceptions.</summary>
        ///
        /// <value>The warning exceptions.</value>
        public List<Exception> WarnExceptions { get; set; }

        /// <summary>Gets or sets the error entries.</summary>
        ///
        /// <value>The error entries.</value>
        public List<string> ErrorEntries { get; set; }

        /// <summary>Gets or sets the error exceptions.</summary>
        ///
        /// <value>The error exceptions.</value>
        public List<Exception> ErrorExceptions { get; set; }

        /// <summary>Gets or sets the fatal entries.</summary>
        ///
        /// <value>The fatal entries.</value>
        public List<string> FatalEntries { get; set; }

        /// <summary>Gets or sets the fatal exceptions.</summary>
        ///
        /// <value>The fatal exceptions.</value>
        public List<Exception> FatalExceptions { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Support.InMemoryLog class.</summary>
        ///
        /// <param name="loggerName">Name of the logger.</param>
        public InMemoryLog(string loggerName)
        {
            this.LoggerName = loggerName;
            this.CombinedLog = new StringBuilder();

            this.DebugEntries = new List<string>();
            this.DebugExceptions = new List<Exception>();
            this.InfoEntries = new List<string>();
            this.InfoExceptions = new List<Exception>();
            this.WarnEntries = new List<string>();
            this.WarnExceptions = new List<Exception>();
            this.ErrorEntries = new List<string>();
            this.ErrorExceptions = new List<Exception>();
            this.FatalEntries = new List<string>();
            this.FatalExceptions = new List<Exception>();
        }

        /// <summary>Gets a value indicating whether this object has exceptions.</summary>
        ///
        /// <value>true if this object has exceptions, false if not.</value>
        public bool HasExceptions
        {
            get
            {
                return this.DebugExceptions.Count > 0
                       || this.InfoExceptions.Count > 0
                       || this.WarnExceptions.Count > 0
                       || this.ErrorExceptions.Count > 0
                       || this.FatalExceptions.Count > 0;
            }
        }

        private void AppendToLog(ICollection<string> logEntries, string format, params object[] args)
        {
            if (format == null) return;
            AppendToLog(logEntries, string.Format(format, args));
        }

        private void AppendToLog(ICollection<string> logEntries, object message)
        {
            if (message == null) return;
            AppendToLog(logEntries, message.ToString());
        }

        private void AppendToLog(
            ICollection<string> logEntries, 
            ICollection<Exception> logExceptions, 
            object message, Exception ex)
        {
            if (ex != null)
            {
                lock (syncLock)
                {
                    logExceptions.Add(ex);
                }
            }
            if (message == null) return;
            AppendToLog(logEntries, message.ToString());
        }

        private void AppendToLog(ICollection<string> logEntries, string message)
        {
            lock (this)
            {
                logEntries.Add(message);
                CombinedLog.AppendLine(message);
            }
        }

        /// <summary>Logs a Debug message.</summary>
        ///
        /// <param name="message">The message.</param>
        public void Debug(object message)
        {
            AppendToLog(DebugEntries, message);
        }

        /// <summary>Logs a Debug message and exception.</summary>
        ///
        /// <param name="message">  The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(object message, Exception exception)
        {
            AppendToLog(DebugEntries, DebugExceptions, message, exception);
        }

        /// <summary>Logs a Debug format message.</summary>
        ///
        /// <param name="format">The format.</param>
        /// <param name="args">  The args.</param>
        public void DebugFormat(string format, params object[] args)
        {
            AppendToLog(DebugEntries, format, args);
        }

        /// <summary>Logs a Error message.</summary>
        ///
        /// <param name="message">The message.</param>
        public void Error(object message)
        {
            AppendToLog(ErrorEntries, message);
        }

        /// <summary>Logs a Error message and exception.</summary>
        ///
        /// <param name="message">  The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(object message, Exception exception)
        {
            AppendToLog(ErrorEntries, ErrorExceptions, message, exception);
        }

        /// <summary>Logs a Error format message.</summary>
        ///
        /// <param name="format">The format.</param>
        /// <param name="args">  The args.</param>
        public void ErrorFormat(string format, params object[] args)
        {
            AppendToLog(ErrorEntries, format, args);
        }

        /// <summary>Logs a Fatal message.</summary>
        ///
        /// <param name="message">The message.</param>
        public void Fatal(object message)
        {
            AppendToLog(FatalEntries, message);
        }

        /// <summary>Logs a Fatal message and exception.</summary>
        ///
        /// <param name="message">  The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(object message, Exception exception)
        {
            AppendToLog(FatalEntries, FatalExceptions, message, exception);
        }

        /// <summary>Logs a Error format message.</summary>
        ///
        /// <param name="format">The format.</param>
        /// <param name="args">  The args.</param>
        public void FatalFormat(string format, params object[] args)
        {
            AppendToLog(FatalEntries, format, args);
        }

        /// <summary>Logs an Info message and exception.</summary>
        ///
        /// <param name="message">The message.</param>
        public void Info(object message)
        {
            AppendToLog(InfoEntries, message);
        }

        /// <summary>Logs an Info message and exception.</summary>
        ///
        /// <param name="message">  The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(object message, Exception exception)
        {
            AppendToLog(InfoEntries, InfoExceptions, message, exception);
        }

        /// <summary>Logs an Info format message.</summary>
        ///
        /// <param name="format">The format.</param>
        /// <param name="args">  The args.</param>
        public void InfoFormat(string format, params object[] args)
        {
            AppendToLog(InfoEntries, format, args);
        }

        /// <summary>Logs a Warning message.</summary>
        ///
        /// <param name="message">The message.</param>
        public void Warn(object message)
        {
            AppendToLog(WarnEntries, message);
        }

        /// <summary>Logs a Warning message and exception.</summary>
        ///
        /// <param name="message">  The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(object message, Exception exception)
        {
            AppendToLog(WarnEntries, WarnExceptions, message, exception);
        }

        /// <summary>Logs a Warning format message.</summary>
        ///
        /// <param name="format">The format.</param>
        /// <param name="args">  The args.</param>
        public void WarnFormat(string format, params object[] args)
        {
            AppendToLog(WarnEntries, format, args);
        }

        /// <summary>Gets a value indicating whether this instance is debug enabled.</summary>
        ///
        /// <value><c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.</value>
        public bool IsDebugEnabled
        {
            get { return true; }
        }
    }
}