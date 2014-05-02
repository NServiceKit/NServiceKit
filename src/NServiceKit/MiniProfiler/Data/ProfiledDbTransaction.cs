using System;
using System.Data.Common;
using System.Data;

#pragma warning disable 1591 // xml doc comments warnings

namespace NServiceKit.MiniProfiler.Data
{
    /// <summary>A profiled database transaction.</summary>
    public class ProfiledDbTransaction : DbTransaction
    {
        private ProfiledDbConnection _conn;
        private DbTransaction _trans;

        /// <summary>Initializes a new instance of the NServiceKit.MiniProfiler.Data.ProfiledDbTransaction class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="transaction">The transaction.</param>
        /// <param name="connection"> The connection.</param>
        public ProfiledDbTransaction(DbTransaction transaction, ProfiledDbConnection connection)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (connection == null) throw new ArgumentNullException("connection");
            this._trans = transaction;
            this._conn = connection;
        }

        /// <summary>Specifies the <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction.</summary>
        ///
        /// <value>The <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction.</value>
        protected override DbConnection DbConnection
        {
            get { return _conn; }
        }

        internal DbTransaction WrappedTransaction
        {
            get { return _trans; }
        }

        /// <summary>Specifies the <see cref="T:System.Data.IsolationLevel" /> for this transaction.</summary>
        ///
        /// <value>The <see cref="T:System.Data.IsolationLevel" /> for this transaction.</value>
        public override IsolationLevel IsolationLevel
        {
            get { return _trans.IsolationLevel; }
        }

        /// <summary>Commits the database transaction.</summary>
        public override void Commit()
        {
            _trans.Commit();
        }

        /// <summary>Rolls back a transaction from a pending state.</summary>
        public override void Rollback()
        {
            _trans.Rollback();
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Data.Common.DbTransaction" /> and optionally releases the managed resources.</summary>
        ///
        /// <param name="disposing">If true, this method releases all resources held by any managed objects that this <see cref="T:System.Data.Common.DbTransaction" /> references.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _trans != null)
            {
                _trans.Dispose();
            }
            _trans = null;
            _conn = null;
            base.Dispose(disposing);
        }
    }
}

#pragma warning restore 1591 // xml doc comments warnings