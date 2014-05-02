using System.Threading;
using NServiceKit.DesignPatterns.Command;

namespace NServiceKit.Common.Support
{
    /// <summary>A command execs handler.</summary>
    public class CommandExecsHandler : ICommandExec
    {
        private readonly ICommandExec command;
        private readonly AutoResetEvent waitHandle;

        /// <summary>Initializes a new instance of the NServiceKit.Common.Support.CommandExecsHandler class.</summary>
        ///
        /// <param name="command">   The command.</param>
        /// <param name="waitHandle">Handle of the wait.</param>
        public CommandExecsHandler(ICommandExec command, AutoResetEvent waitHandle)
        {
            this.command = command;
            this.waitHandle = waitHandle;
        }

        /// <summary>Executes this object.</summary>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool Execute()
        {
            command.Execute();
            waitHandle.Set();
            return true;
        }
    }
}