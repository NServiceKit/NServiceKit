namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for cache text manager factory.</summary>
	public interface ICacheTextManagerFactory
	{
        /// <summary>Resolves.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>An ICacheTextManager.</returns>
		ICacheTextManager Resolve(string contentType);
	}
}