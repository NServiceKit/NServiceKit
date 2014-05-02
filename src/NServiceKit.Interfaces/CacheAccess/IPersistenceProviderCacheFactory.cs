using NServiceKit.DataAccess;

namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for persistence provider cache factory.</summary>
	public interface IPersistenceProviderCacheFactory
	{
        /// <summary>Creates a new IPersistenceProviderCache.</summary>
        ///
        /// <param name="providerManager">Manager for provider.</param>
        ///
        /// <returns>An IPersistenceProviderCache.</returns>
		IPersistenceProviderCache Create(IPersistenceProviderManager providerManager);

        /// <summary>Creates a new IPersistenceProviderCache.</summary>
        ///
        /// <param name="conntectionString">The conntection string.</param>
        ///
        /// <returns>An IPersistenceProviderCache.</returns>
		IPersistenceProviderCache Create(string conntectionString);
	}
}