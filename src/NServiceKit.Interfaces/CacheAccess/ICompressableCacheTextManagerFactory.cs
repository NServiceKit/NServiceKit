namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for compressable cache text manager factory.</summary>
	public interface ICompressableCacheTextManagerFactory
	{
        /// <summary>Resolves.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>An ICompressableCacheTextManager.</returns>
		ICompressableCacheTextManager Resolve(string contentType);
	}
}