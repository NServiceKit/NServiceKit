using NServiceKit.DataAccess;

namespace NServiceKit.CacheAccess
{
	public interface IPersistenceProviderCacheFactory
	{
		IPersistenceProviderCache Create(IPersistenceProviderManager providerManager);
		
		IPersistenceProviderCache Create(string conntectionString);
	}
}