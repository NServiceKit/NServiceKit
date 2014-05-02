using NServiceKit.DataAccess;

namespace NServiceKit.CacheAccess.Providers
{
    /// <summary>A persistence provider cache.</summary>
	public class PersistenceProviderCache : BasicPersistenceProviderCacheBase
	{
        /// <summary>Gets or sets the manager for provider.</summary>
        ///
        /// <value>The provider manager.</value>
		protected IPersistenceProviderManager ProviderManager { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.CacheAccess.Providers.PersistenceProviderCache class.</summary>
        ///
        /// <param name="cacheClient">    The cache client.</param>
        /// <param name="providerManager">Manager for provider.</param>
		public PersistenceProviderCache(ICacheClient cacheClient, IPersistenceProviderManager providerManager)
			: base(cacheClient)
		{
			this.ProviderManager = providerManager;
		}

        /// <summary>Gets basic persistence provider.</summary>
        ///
        /// <returns>The basic persistence provider.</returns>
		public override IBasicPersistenceProvider GetBasicPersistenceProvider()
		{
			return this.ProviderManager.GetProvider();
		}
	}
}