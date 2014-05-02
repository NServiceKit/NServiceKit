using System;

namespace NServiceKit.DataAccess
{
	/// <summary>
	/// Manages a connection to a persistance provider
	/// </summary>
	public interface IPersistenceProviderManager : IDisposable
	{
        /// <summary>Gets the provider.</summary>
        ///
        /// <returns>The provider.</returns>
		IPersistenceProvider GetProvider();
	}
}