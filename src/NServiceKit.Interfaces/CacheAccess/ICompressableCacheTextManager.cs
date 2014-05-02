using System;

namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for compressable cache text manager.</summary>
	public interface ICompressableCacheTextManager
		: IHasCacheClient, ICacheHasContentType, ICacheClearable
	{
        /// <summary>Resolves.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="compressionType">Type of the compression.</param>
        /// <param name="cacheKey">       The cache key.</param>
        /// <param name="createCacheFn">  The create cache function.</param>
        ///
        /// <returns>An object.</returns>
		object Resolve<T>(string compressionType, string cacheKey, Func<T> createCacheFn) 
			where T : class;
	}
}