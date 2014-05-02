namespace NServiceKit.ServiceInterface.ServiceModel
{
    /// <summary>Interface for cache by etag.</summary>
	public interface ICacheByEtag
	{
        /// <summary>Gets the etag.</summary>
        ///
        /// <value>The etag.</value>
		string Etag { get; }
	}
}