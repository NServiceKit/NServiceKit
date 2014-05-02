using System;

namespace NServiceKit.LogicFacade
{
    /// <summary>Interface for initialise context.</summary>
	public interface IInitContext : IDisposable
	{
        /// <summary>Gets the initialised object.</summary>
        ///
        /// <value>The initialised object.</value>
		object InitialisedObject
		{
			get;
		}
	}
}