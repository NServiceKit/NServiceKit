using System;

namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for cache text manager.</summary>
	public interface ICacheTextManager 
		: IHasCacheClient, ICacheClearable
	{
        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		string ContentType { get; }

        /// <summary>Resolve text.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="cacheKey">     The cache key.</param>
        /// <param name="createCacheFn">The create cache function.</param>
        ///
        /// <returns>A string.</returns>
		string ResolveText<T>(string cacheKey, Func<T> createCacheFn)
			where T : class;

        /// <summary>Resolve text.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="cacheKey">     The cache key.</param>
        /// <param name="expiresIn">    The expires in.</param>
        /// <param name="createCacheFn">The create cache function.</param>
        ///
        /// <returns>A string.</returns>
		string ResolveText<T>(string cacheKey, TimeSpan expiresIn, Func<T> createCacheFn)
			where T : class;
	}
}