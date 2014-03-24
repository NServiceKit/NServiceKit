namespace NServiceKit.DataAccess
{
	public interface IPersistenceProviderManagerFactory
	{
		IPersistenceProviderManager CreateProviderManager(string connectionString);
	}
}