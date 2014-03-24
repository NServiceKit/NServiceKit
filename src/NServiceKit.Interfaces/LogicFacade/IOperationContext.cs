using System;
using NServiceKit.ServiceHost;

namespace NServiceKit.LogicFacade
{
	public interface IOperationContext : IDisposable
	{
		IApplicationContext Application { get;  }
		
		IRequestContext Request { get; }
	}
}