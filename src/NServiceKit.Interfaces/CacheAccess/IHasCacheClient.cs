namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for has cache client.</summary>
	public interface IHasCacheClient
	{
        /// <summary>Gets the cache client.</summary>
        ///
        /// <value>The cache client.</value>
		ICacheClient CacheClient { get; }
	}
}