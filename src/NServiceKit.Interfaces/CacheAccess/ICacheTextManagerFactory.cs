namespace NServiceKit.CacheAccess
{
	public interface ICacheTextManagerFactory
	{
		ICacheTextManager Resolve(string contentType);
	}
}