namespace NServiceKit.CacheAccess
{
	public interface ICompressableCacheTextManagerFactory
	{
		ICompressableCacheTextManager Resolve(string contentType);
	}
}