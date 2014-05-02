namespace NServiceKit.DataAccess
{
    /// <summary>Interface for persistence provider manager factory.</summary>
	public interface IPersistenceProviderManagerFactory
	{
        /// <summary>Creates provider manager.</summary>
        ///
        /// <param name="connectionString">The connection string.</param>
        ///
        /// <returns>The new provider manager.</returns>
		IPersistenceProviderManager CreateProviderManager(string connectionString);
	}
}