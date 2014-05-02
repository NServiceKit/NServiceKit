namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for cache has content type.</summary>
	public interface ICacheHasContentType
	{
        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		string ContentType { get; }
	}
}