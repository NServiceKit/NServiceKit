namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for deflate provider.</summary>
	public interface IDeflateProvider
	{
        /// <summary>Deflates.</summary>
        ///
        /// <param name="text">The text.</param>
        ///
        /// <returns>A byte[].</returns>
		byte[] Deflate(string text);

        /// <summary>Inflates the given gz buffer.</summary>
        ///
        /// <param name="gzBuffer">Buffer for gz data.</param>
        ///
        /// <returns>A string.</returns>
		string Inflate(byte[] gzBuffer);
	}
}
