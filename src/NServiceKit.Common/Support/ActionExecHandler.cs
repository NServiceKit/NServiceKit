using System;
using System.Threading;
using NServiceKit.DesignPatterns.Command;

namespace NServiceKit.Common.Support
{
    /// <summary>An action execute handler.</summary>
    public class ActionExecHandler : ICommandExec
    {
        private readonly Action action;
        private readonly AutoResetEvent waitHandle;

        /// <summary>Initializes a new instance of the NServiceKit.Common.Support.ActionExecHandler class.</summary>
        ///
        /// <param name="action">    The action.</param>
        /// <param name="waitHandle">Handle of the wait.</param>
        public ActionExecHandler(Action action, AutoResetEvent waitHandle)
        {
            this.action = action;
            this.waitHandle = waitHandle;
        }

        /// <summary>Executes this object.</summary>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool Execute()
        {
            action();
            waitHandle.Set();
            return true;
        }
    }
}