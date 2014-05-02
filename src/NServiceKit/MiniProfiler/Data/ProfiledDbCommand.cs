using System;
using System.Data.Common;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

#pragma warning disable 1591 // xml doc comments warnings

namespace NServiceKit.MiniProfiler.Data
{
    /// <summary>A profiled database command.</summary>
    public class ProfiledDbCommand : DbCommand, ICloneable
    {
        /// <summary>The command.</summary>
        protected DbCommand _cmd;
        /// <summary>The connection.</summary>
        protected DbConnection _conn;
        /// <summary>The tran.</summary>
        protected DbTransaction _tran;
        /// <summary>The profiler.</summary>
        protected IDbProfiler _profiler;

        private bool bindByName;
        /// <summary>
        /// If the underlying command supports BindByName, this sets/clears the underlying
        /// implementation accordingly. This is required to support OracleCommand from dapper-dot-net
        /// </summary>
        public bool BindByName
        {
            get { return bindByName; }
            set
            {
                if (bindByName != value)
                {
                    if (_cmd != null)
                    {
                        var inner = GetBindByName(_cmd.GetType());
                        if (inner != null) inner(_cmd, value);
                    }
                    bindByName = value;
                }
            }
        }
        static Link<Type, Action<IDbCommand, bool>> bindByNameCache;
        static Action<IDbCommand, bool> GetBindByName(Type commandType)
        {
            if (commandType == null) return null; // GIGO
            Action<IDbCommand, bool> action;
            if (Link<Type, Action<IDbCommand, bool>>.TryGet(bindByNameCache, commandType, out action))
            {
                return action;
            }
            var prop = commandType.GetProperty("BindByName", BindingFlags.Public | BindingFlags.Instance);
            action = null;
            ParameterInfo[] indexers;
            MethodInfo setter;
            if (prop != null && prop.CanWrite && prop.PropertyType == typeof(bool)
                && ((indexers = prop.GetIndexParameters()) == null || indexers.Length == 0)
                && (setter = prop.GetSetMethod()) != null
                )
            {
                var method = new DynamicMethod(commandType.Name + "_BindByName", null, new Type[] { typeof(IDbCommand), typeof(bool) });
                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, commandType);
                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(OpCodes.Callvirt, setter, null);
                il.Emit(OpCodes.Ret);
                action = (Action<IDbCommand, bool>)method.CreateDelegate(typeof(Action<IDbCommand, bool>));
            }
            // cache it            
            Link<Type, Action<IDbCommand, bool>>.TryAdd(ref bindByNameCache, commandType, ref action);
            return action;
        }

        /// <summary>Initializes a new instance of the NServiceKit.MiniProfiler.Data.ProfiledDbCommand class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="cmd">     The command.</param>
        /// <param name="conn">    The connection.</param>
        /// <param name="profiler">The profiler.</param>
        public ProfiledDbCommand(DbCommand cmd, DbConnection conn, IDbProfiler profiler)
        {
            if (cmd == null) throw new ArgumentNullException("cmd");

            _cmd = cmd;
            _conn = conn;

            if (profiler != null)
            {
                _profiler = profiler;
            }
        }

        /// <summary>Gets or sets the text command to run against the data source.</summary>
        ///
        /// <value>The text command to execute. The default value is an empty string ("").</value>
        public override string CommandText
        {
            get { return _cmd.CommandText; }
            set { _cmd.CommandText = value; }
        }

        /// <summary>Gets or sets the wait time before terminating the attempt to execute a command and generating an error.</summary>
        ///
        /// <value>The time in seconds to wait for the command to execute.</value>
        public override int CommandTimeout
        {
            get { return _cmd.CommandTimeout; }
            set { _cmd.CommandTimeout = value; }
        }

        /// <summary>Indicates or specifies how the <see cref="P:System.Data.Common.DbCommand.CommandText" /> property is interpreted.</summary>
        ///
        /// <value>One of the <see cref="T:System.Data.CommandType" /> values. The default is Text.</value>
        public override CommandType CommandType
        {
            get { return _cmd.CommandType; }
            set { _cmd.CommandType = value; }
        }

        /// <summary>Gets or sets the <see cref="T:System.Data.Common.DbConnection" /> used by this <see cref="T:System.Data.Common.DbCommand" />.</summary>
        ///
        /// <value>The connection to the data source.</value>
        protected override DbConnection DbConnection
        {
            get { return _conn; }
            set
            {
                // TODO: we need a way to grab the IDbProfiler which may not be the same as the MiniProfiler, it could be wrapped
                // allow for command reuse, it is clear the connection is going to need to be reset
                if (Profiler.Current != null)
                {
                    _profiler = Profiler.Current;
                }

                _conn = value;
                var awesomeConn = value as ProfiledDbConnection;
                _cmd.Connection = awesomeConn == null ? value : awesomeConn.WrappedConnection;
            }
        }

        /// <summary>Gets the collection of <see cref="T:System.Data.Common.DbParameter" /> objects.</summary>
        ///
        /// <value>The parameters of the SQL statement or stored procedure.</value>
        protected override DbParameterCollection DbParameterCollection
        {
            get { return _cmd.Parameters; }
        }

        /// <summary>Gets or sets the <see cref="P:System.Data.Common.DbCommand.DbTransaction" /> within which this <see cref="T:System.Data.Common.DbCommand" /> object executes.</summary>
        ///
        /// <value>The transaction within which a Command object of a .NET Framework data provider executes. The default value is a null reference (Nothing in Visual Basic).</value>
        protected override DbTransaction DbTransaction
        {
            get { return _tran; }
            set
            {
                this._tran = value;
                var awesomeTran = value as ProfiledDbTransaction;
                _cmd.Transaction = awesomeTran == null ? value : awesomeTran.WrappedTransaction;
            }
        }

        /// <summary>Gets or sets a value indicating whether the command object should be visible in a customized interface control.</summary>
        ///
        /// <value>true, if the command object should be visible in a control; otherwise false. The default is true.</value>
        public override bool DesignTimeVisible
        {
            get { return _cmd.DesignTimeVisible; }
            set { _cmd.DesignTimeVisible = value; }
        }

        /// <summary>
        /// Gets or sets how command results are applied to the <see cref="T:System.Data.DataRow" /> when used by the Update method of a <see cref="T:System.Data.Common.DbDataAdapter" />.
        /// </summary>
        ///
        /// <value>One of the <see cref="T:System.Data.UpdateRowSource" /> values. The default is Both unless the command is automatically generated. Then the default is None.</value>
        public override UpdateRowSource UpdatedRowSource
        {
            get { return _cmd.UpdatedRowSource; }
            set { _cmd.UpdatedRowSource = value; }
        }

        /// <summary>Executes the command text against the connection.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="behavior">An instance of <see cref="T:System.Data.CommandBehavior" />.</param>
        ///
        /// <returns>A <see cref="T:System.Data.Common.DbDataReader" />.</returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            if (_profiler == null || !_profiler.IsActive)
            {
                return _cmd.ExecuteReader(behavior);
            }

            DbDataReader result = null;
            _profiler.ExecuteStart(this, ExecuteType.Reader);
            try
            {
                result = _cmd.ExecuteReader(behavior);
                result = new ProfiledDbDataReader(result, _conn, _profiler);
            }
            catch (Exception e)
            {
                _profiler.OnError(this, ExecuteType.Reader, e);
                throw;
            }
            finally
            {
                _profiler.ExecuteFinish(this, ExecuteType.Reader, result);
            }
            return result;
        }

        /// <summary>Executes a SQL statement against a connection object.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <returns>The number of rows affected.</returns>
        public override int ExecuteNonQuery()
        {
            if (_profiler == null || !_profiler.IsActive)
            {
                return _cmd.ExecuteNonQuery();
            }

            int result;

            _profiler.ExecuteStart(this, ExecuteType.NonQuery);
            try
            {
                result = _cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                _profiler.OnError(this, ExecuteType.NonQuery, e);
                throw;
            }
            finally
            {
                _profiler.ExecuteFinish(this, ExecuteType.NonQuery, null);
            }
            return result;
        }

        /// <summary>Executes the query and returns the first column of the first row in the result set returned by the query. All other columns and rows are ignored.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <returns>The first column of the first row in the result set.</returns>
        public override object ExecuteScalar()
        {
            if (_profiler == null || !_profiler.IsActive)
            {
                return _cmd.ExecuteScalar();
            }

            object result;
            _profiler.ExecuteStart(this, ExecuteType.Scalar);
            try
            {
                result = _cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                _profiler.OnError(this, ExecuteType.Scalar, e);
                throw;
            }
            finally
            {
                _profiler.ExecuteFinish(this, ExecuteType.Scalar, null);
            }
            return result;
        }

        /// <summary>Attempts to cancels the execution of a <see cref="T:System.Data.Common.DbCommand" />.</summary>
        public override void Cancel()
        {
            _cmd.Cancel();
        }

        /// <summary>Creates a prepared (or compiled) version of the command on the data source.</summary>
        public override void Prepare()
        {
            _cmd.Prepare();
        }

        /// <summary>Creates a new instance of a <see cref="T:System.Data.Common.DbParameter" /> object.</summary>
        ///
        /// <returns>A <see cref="T:System.Data.Common.DbParameter" /> object.</returns>
        protected override DbParameter CreateDbParameter()
        {
            return _cmd.CreateParameter();
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component" /> and optionally releases the managed resources.</summary>
        ///
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _cmd != null)
            {
                _cmd.Dispose();
            }
            _cmd = null;
            base.Dispose(disposing);
        }

        /// <summary>Makes a deep copy of this object.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <returns>A copy of this object.</returns>
        public ProfiledDbCommand Clone()
        { // EF expects ICloneable
            ICloneable tail = _cmd as ICloneable;
            if (tail == null) throw new NotSupportedException("Underlying " + _cmd.GetType().Name + " is not cloneable");
            return new ProfiledDbCommand((DbCommand)tail.Clone(), _conn, _profiler);
        }
        object ICloneable.Clone() { return Clone(); }

    }
}

#pragma warning restore 1591 // xml doc comments warnings