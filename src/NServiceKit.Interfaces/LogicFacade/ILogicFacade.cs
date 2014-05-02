using System;

namespace NServiceKit.LogicFacade
{
    /// <summary>Interface for logic facade.</summary>
	public interface ILogicFacade : IDisposable
	{
        /// <summary>Acquires the initialise context described by options.</summary>
        ///
        /// <param name="options">Options for controlling the operation.</param>
        ///
        /// <returns>An IInitContext.</returns>
		IInitContext AcquireInitContext(InitOptions options);
	}
}