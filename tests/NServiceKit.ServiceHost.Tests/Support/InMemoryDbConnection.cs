using System.Data;
using NServiceKit.OrmLite;

namespace NServiceKit.ServiceHost.Tests.Support
{
	/// <summary>
	/// LAMO hack I'm forced to do to because I can't register a simple delegate 
	/// to create my instance type
	/// </summary>
	public class InMemoryDbConnection 
		: IDbConnection
	{
		private readonly IDbConnection inner;
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Support.InMemoryDbConnection class.</summary>
		public InMemoryDbConnection()
		{
			this.inner = ":memory:".OpenDbConnection();
		}

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			this.inner.Dispose();
		}

        /// <summary>Begins a database transaction.</summary>
        ///
        /// <returns>An object representing the new transaction.</returns>
		public IDbTransaction BeginTransaction()
		{
			return this.inner.BeginTransaction();
		}

        /// <summary>Begins a database transaction with the specified <see cref="T:System.Data.IsolationLevel" /> value.</summary>
        ///
        /// <param name="il">One of the <see cref="T:System.Data.IsolationLevel" /> values.</param>
        ///
        /// <returns>An object representing the new transaction.</returns>
		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return this.inner.BeginTransaction(il);
		}

        /// <summary>Closes the connection to the database.</summary>
		public void Close()
		{
			this.inner.Close();
		}

        /// <summary>Changes the current database for an open Connection object.</summary>
        ///
        /// <param name="databaseName">The name of the database to use in place of the current database.</param>
		public void ChangeDatabase(string databaseName)
		{
			this.inner.ChangeDatabase(databaseName);
		}

        /// <summary>Creates and returns a Command object associated with the connection.</summary>
        ///
        /// <returns>A Command object associated with the connection.</returns>
		public IDbCommand CreateCommand()
		{
			return this.inner.CreateCommand();
		}

        /// <summary>Opens a database connection with the settings specified by the ConnectionString property of the provider-specific Connection object.</summary>
		public void Open()
		{
			this.inner.Open();
		}

        /// <summary>Gets or sets the string used to open a database.</summary>
        ///
        /// <value>A string containing connection settings.</value>
		public string ConnectionString
		{
			get { return this.inner.ConnectionString; }
			set { this.inner.ConnectionString = value; }
		}

        /// <summary>Gets the time to wait while trying to establish a connection before terminating the attempt and generating an error.</summary>
        ///
        /// <value>The time (in seconds) to wait for a connection to open. The default value is 15 seconds.</value>
		public int ConnectionTimeout
		{
			get { return this.inner.ConnectionTimeout; }
		}

        /// <summary>Gets the name of the current database or the database to be used after a connection is opened.</summary>
        ///
        /// <value>The name of the current database or the name of the database to be used once a connection is open. The default value is an empty string.</value>
		public string Database
		{
			get { return this.inner.Database; }
		}

        /// <summary>Gets the current state of the connection.</summary>
        ///
        /// <value>One of the <see cref="T:System.Data.ConnectionState" /> values.</value>
		public ConnectionState State
		{
			get { return this.inner.State; }
		}
	}
}