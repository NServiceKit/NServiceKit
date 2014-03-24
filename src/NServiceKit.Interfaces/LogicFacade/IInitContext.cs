using System;

namespace NServiceKit.LogicFacade
{
	public interface IInitContext : IDisposable
	{
		object InitialisedObject
		{
			get;
		}
	}
}