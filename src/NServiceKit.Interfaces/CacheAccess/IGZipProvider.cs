namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for ig zip provider.</summary>
	public interface IGZipProvider
	{
        /// <summary>Zips.</summary>
        ///
        /// <param name="text">The text.</param>
        ///
        /// <returns>A byte[].</returns>
		byte[] GZip(string text);

        /// <summary>Unzips the given gz buffer.</summary>
        ///
        /// <param name="gzBuffer">Buffer for gz data.</param>
        ///
        /// <returns>A string.</returns>
		string GUnzip(byte[] gzBuffer);
	}
}