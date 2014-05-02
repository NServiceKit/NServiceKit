using System;

namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for cache manager.</summary>
	public interface ICacheManager
		: ICacheClearable, IHasCacheClient
	{
        /// <summary>Resolves.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="cacheKey">     The cache key.</param>
        /// <param name="createCacheFn">The create cache function.</param>
        ///
        /// <returns>A T.</returns>
		T Resolve<T>(string cacheKey, Func<T> createCacheFn)
			where T : class;

        /// <summary>Resolves.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="cacheKey">     The cache key.</param>
        /// <param name="expireIn">     The expire in.</param>
        /// <param name="createCacheFn">The create cache function.</param>
        ///
        /// <returns>A T.</returns>
		T Resolve<T>(string cacheKey, TimeSpan expireIn, Func<T> createCacheFn)
			where T : class;
	}
}