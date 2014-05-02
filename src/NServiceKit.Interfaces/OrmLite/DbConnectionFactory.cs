using System;
using System.Data;

namespace NServiceKit.OrmLite
{
    /// <summary>A database connection factory.</summary>
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly Func<IDbConnection> connectionFactoryFn;

        /// <summary>Initializes a new instance of the NServiceKit.OrmLite.DbConnectionFactory class.</summary>
        ///
        /// <param name="connectionFactoryFn">The connection factory function.</param>
        public DbConnectionFactory(Func<IDbConnection> connectionFactoryFn)
        {
            this.connectionFactoryFn = connectionFactoryFn;
        }

        /// <summary>Opens database connection.</summary>
        ///
        /// <returns>An IDbConnection.</returns>
        public IDbConnection OpenDbConnection()
        {
            var dbConn = CreateDbConnection();
            dbConn.Open();
            return dbConn;
        }

        /// <summary>Creates database connection.</summary>
        ///
        /// <returns>The new database connection.</returns>
        public IDbConnection CreateDbConnection()
        {
            return connectionFactoryFn();
        }
    }
}