using System.Data;

namespace NServiceKit.OrmLite
{
    /// <summary>Interface for database connection factory.</summary>
    public interface IDbConnectionFactory
    {
        /// <summary>Opens database connection.</summary>
        ///
        /// <returns>An IDbConnection.</returns>
        IDbConnection OpenDbConnection();

        /// <summary>Creates database connection.</summary>
        ///
        /// <returns>The new database connection.</returns>
        IDbConnection CreateDbConnection();
    }
}