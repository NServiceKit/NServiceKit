using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using NServiceKit.DataAccess;

namespace NServiceKit.MiniProfiler.Data
{
    /// <summary>
    /// Wraps a database connection, allowing sql execution timings to be collected when a <see cref="MiniProfiler.Profiler"/> session is started.
    /// </summary>
    public class ProfiledDbConnection : DbConnection, ICloneable
    {
        /// <summary>
        /// This will be made private; use <see cref="InnerConnection"/>
        /// </summary>
        protected DbConnection _conn; // TODO: in MiniProfiler 2.0, make private

        /// <summary>true to automatically dispose connection.</summary>
        protected bool autoDisposeConnection;

        /// <summary>
        /// The underlying, real database connection to your db provider.
        /// </summary>
        public DbConnection InnerConnection
        {
            get { return _conn; }
        }

        /// <summary>
        /// This will be made private; use <see cref="Profiler"/>
        /// </summary>
        protected IDbProfiler _profiler; // TODO: in MiniProfiler 2.0, make private
        /// <summary>
        /// The current profiler instance; could be null.
        /// </summary>
        public IDbProfiler Profiler
        {
            get { return _profiler; }
        }

        /// <summary>
        /// Returns a new <see cref="ProfiledDbConnection"/> that wraps <paramref name="connection"/>, 
        /// providing query execution profiling.  If profiler is null, no profiling will occur.
        /// </summary>
        /// <param name="connection">Your provider-specific flavor of connection, e.g. SqlConnection, OracleConnection</param>
        /// <param name="profiler">The currently started <see cref="MiniProfiler.Profiler"/> or null.</param>
        /// <param name="autoDisposeConnection">Determines whether the ProfiledDbConnection will dispose the underlying connection.</param>
        public ProfiledDbConnection(DbConnection connection, IDbProfiler profiler, bool autoDisposeConnection = true)
        {
        	Init(connection, profiler, autoDisposeConnection);
        }

        private void Init(DbConnection connection, IDbProfiler profiler, bool autoDisposeConnection)
    	{
    		if (connection == null) throw new ArgumentNullException("connection");

    	    this.autoDisposeConnection = autoDisposeConnection;
    		_conn = connection;
    		_conn.StateChange += StateChangeHandler;

    		if (profiler != null)
    		{
    			_profiler = profiler;
    		}
    	}

        /// <summary>Returns a new <see cref="ProfiledDbConnection"/> that wraps <paramref name="connection"/>, providing query execution profiling.  If profiler is null, no profiling will occur.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="connection">           Your provider-specific flavor of connection, e.g. SqlConnection, OracleConnection.</param>
        /// <param name="profiler">             The currently started <see cref="MiniProfiler.Profiler"/> or null.</param>
        /// <param name="autoDisposeConnection">Determines whether the ProfiledDbConnection will dispose the underlying connection.</param>
        public ProfiledDbConnection(IDbConnection connection, IDbProfiler profiler, bool autoDisposeConnection=true)
        {
    		var hasConn = connection as IHasDbConnection;
			if (hasConn != null) connection = hasConn.DbConnection;
    		var dbConn = connection as DbConnection;

			if (dbConn == null)
				throw new ArgumentException(connection.GetType().Name + " does not inherit DbConnection");
			
			Init(dbConn, profiler, autoDisposeConnection);
        }


#pragma warning disable 1591 // xml doc comments warnings


        /// <summary>
        /// The raw connection this is wrapping
        /// </summary>
        public DbConnection WrappedConnection
        {
            get { return _conn; }
        }

        /// <summary>Gets a value indicating whether the component can raise an event.</summary>
        ///
        /// <value>true if the component can raise events; otherwise, false. The default is true.</value>
        protected override bool CanRaiseEvents
        {
            get { return true; }
        }

        /// <summary>Gets or sets the string used to open the connection.</summary>
        ///
        /// <value>
        /// The connection string used to establish the initial connection. The exact contents of the connection string depend on the specific data source for this connection. The default value is an empty
        /// string.
        /// </value>
        public override string ConnectionString
        {
            get { return _conn.ConnectionString; }
            set { _conn.ConnectionString = value; }
        }

        /// <summary>Gets the time to wait while establishing a connection before terminating the attempt and generating an error.</summary>
        ///
        /// <value>The time (in seconds) to wait for a connection to open. The default value is determined by the specific type of connection that you are using.</value>
        public override int ConnectionTimeout
        {
            get { return _conn.ConnectionTimeout; }
        }

        /// <summary>Gets the name of the current database after a connection is opened, or the database name specified in the connection string before the connection is opened.</summary>
        ///
        /// <value>The name of the current database or the name of the database to be used after a connection is opened. The default value is an empty string.</value>
        public override string Database
        {
            get { return _conn.Database; }
        }

        /// <summary>Gets the name of the database server to which to connect.</summary>
        ///
        /// <value>The name of the database server to which to connect. The default value is an empty string.</value>
        public override string DataSource
        {
            get { return _conn.DataSource; }
        }

        /// <summary>Gets a string that represents the version of the server to which the object is connected.</summary>
        ///
        /// <value>The version of the database. The format of the string returned depends on the specific type of connection you are using.</value>
        public override string ServerVersion
        {
            get { return _conn.ServerVersion; }
        }

        /// <summary>Gets a string that describes the state of the connection.</summary>
        ///
        /// <value>The state of the connection. The format of the string returned depends on the specific type of connection you are using.</value>
        public override ConnectionState State
        {
            get { return _conn.State; }
        }

        /// <summary>Changes the current database for an open connection.</summary>
        ///
        /// <param name="databaseName">Specifies the name of the database for the connection to use.</param>
        public override void ChangeDatabase(string databaseName)
        {
            _conn.ChangeDatabase(databaseName);
        }

        /// <summary>Closes the connection to the database. This is the preferred method of closing any open connection.</summary>
        public override void Close()
        {
            if (autoDisposeConnection)
                _conn.Close();
        }

		//public override void EnlistTransaction(System.Transactions.Transaction transaction)
		//{
		//    _conn.EnlistTransaction(transaction);
		//}

        /// <summary>Returns schema information for the data source of this <see cref="T:System.Data.Common.DbConnection" />.</summary>
        ///
        /// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
        public override DataTable GetSchema()
        {
            return _conn.GetSchema();
        }

        /// <summary>Returns schema information for the data source of this <see cref="T:System.Data.Common.DbConnection" /> using the specified string for the schema name.</summary>
        ///
        /// <param name="collectionName">Specifies the name of the schema to return.</param>
        ///
        /// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
        public override DataTable GetSchema(string collectionName)
        {
            return _conn.GetSchema(collectionName);
        }

        /// <summary>
        /// Returns schema information for the data source of this <see cref="T:System.Data.Common.DbConnection" /> using the specified string for the schema name and the specified string array for the
        /// restriction values.
        /// </summary>
        ///
        /// <param name="collectionName">   Specifies the name of the schema to return.</param>
        /// <param name="restrictionValues">Specifies a set of restriction values for the requested schema.</param>
        ///
        /// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return _conn.GetSchema(collectionName, restrictionValues);
        }

        /// <summary>Opens a database connection with the settings specified by the <see cref="P:System.Data.Common.DbConnection.ConnectionString" />.</summary>
        public override void Open()
        {
            if (_conn.State != ConnectionState.Open)
                _conn.Open();
        }

        /// <summary>Starts a database transaction.</summary>
        ///
        /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
        ///
        /// <returns>An object representing the new transaction.</returns>
        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return new ProfiledDbTransaction(_conn.BeginTransaction(isolationLevel), this);
        }

        /// <summary>Creates and returns a <see cref="T:System.Data.Common.DbCommand" /> object associated with the current connection.</summary>
        ///
        /// <returns>A <see cref="T:System.Data.Common.DbCommand" /> object.</returns>
        protected override DbCommand CreateDbCommand()
        {
            return new ProfiledDbCommand(_conn.CreateCommand(), this, _profiler);
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component" /> and optionally releases the managed resources.</summary>
        ///
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _conn != null)
            {
                _conn.StateChange -= StateChangeHandler;
                if (autoDisposeConnection)
                    _conn.Dispose();
            }
            _conn = null;
            _profiler = null;
            base.Dispose(disposing);
        }

        void StateChangeHandler(object sender, StateChangeEventArgs e)
        {
            OnStateChange(e);
        }

        /// <summary>Makes a deep copy of this object.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <returns>A copy of this object.</returns>
        public ProfiledDbConnection Clone()
        {
            ICloneable tail = _conn as ICloneable;
            if (tail == null) throw new NotSupportedException("Underlying " + _conn.GetType().Name + " is not cloneable");
            return new ProfiledDbConnection((DbConnection)tail.Clone(), _profiler, autoDisposeConnection);
        }
        object ICloneable.Clone() { return Clone(); }

    }
}

#pragma warning restore 1591 // xml doc comments warnings