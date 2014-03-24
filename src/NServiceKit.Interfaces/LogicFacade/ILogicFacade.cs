using System;

namespace NServiceKit.LogicFacade
{
	public interface ILogicFacade : IDisposable
	{
		IInitContext AcquireInitContext(InitOptions options);
	}
}