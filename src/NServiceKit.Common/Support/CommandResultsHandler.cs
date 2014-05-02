using System.Collections.Generic;
using System.Threading;
using NServiceKit.DesignPatterns.Command;

namespace NServiceKit.Common.Support
{
    /// <summary>A command results handler.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class CommandResultsHandler<T> : ICommandExec
    {
        private readonly List<T> results;
        private readonly ICommandList<T> command;
        private readonly AutoResetEvent waitHandle;

        /// <summary>Initializes a new instance of the NServiceKit.Common.Support.CommandResultsHandler&lt;T&gt; class.</summary>
        ///
        /// <param name="results">   The results.</param>
        /// <param name="command">   The command.</param>
        /// <param name="waitHandle">Handle of the wait.</param>
        public CommandResultsHandler(List<T> results, ICommandList<T> command, AutoResetEvent waitHandle)
        {
            this.results = results;
            this.command = command;
            this.waitHandle = waitHandle;
        }

        /// <summary>Executes this object.</summary>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool Execute()
        {
            results.AddRange(command.Execute());
            waitHandle.Set();
            return true;
        }
    }
}