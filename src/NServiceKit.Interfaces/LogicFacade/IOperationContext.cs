using System;
using NServiceKit.ServiceHost;

namespace NServiceKit.LogicFacade
{
    /// <summary>Interface for operation context.</summary>
	public interface IOperationContext : IDisposable
	{
        /// <summary>Gets the application.</summary>
        ///
        /// <value>The application.</value>
		IApplicationContext Application { get;  }

        /// <summary>Gets the request.</summary>
        ///
        /// <value>The request.</value>
		IRequestContext Request { get; }
	}
}