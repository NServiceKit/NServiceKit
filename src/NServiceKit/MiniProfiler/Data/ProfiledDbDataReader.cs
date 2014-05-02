using System;
using System.Data.Common;
using System.Data;

#pragma warning disable 1591 // xml doc comments warnings

namespace NServiceKit.MiniProfiler.Data
{

    /// <summary>A profiled database data reader.</summary>
    public class ProfiledDbDataReader : DbDataReader
    {

        private DbConnection _conn;
        private DbDataReader _reader;
        private IDbProfiler _profiler;

        /// <summary>Initializes a new instance of the NServiceKit.MiniProfiler.Data.ProfiledDbDataReader class.</summary>
        ///
        /// <param name="reader">    The reader.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="profiler">  The profiler.</param>
        public ProfiledDbDataReader(DbDataReader reader, DbConnection connection, IDbProfiler profiler)
        {
            _reader = reader;
            _conn = connection;

            if (profiler != null)
            {
                _profiler = profiler;
            }
        }

        /// <summary>Gets a value indicating the depth of nesting for the current row.</summary>
        ///
        /// <value>The depth of nesting for the current row.</value>
        public override int Depth
        {
            get { return _reader.Depth; }
        }

        /// <summary>Gets the number of columns in the current row.</summary>
        ///
        /// <value>The number of columns in the current row.</value>
        ///
        /// ### <exception cref="T:System.NotSupportedException">There is no current connection to an instance of SQL Server.</exception>
        public override int FieldCount
        {
            get { return _reader.FieldCount; }
        }

        /// <summary>Gets a value that indicates whether this <see cref="T:System.Data.Common.DbDataReader" /> contains one or more rows.</summary>
        ///
        /// <value>true if the <see cref="T:System.Data.Common.DbDataReader" /> contains one or more rows; otherwise false.</value>
        public override bool HasRows
        {
            get { return _reader.HasRows; }
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Data.Common.DbDataReader" /> is closed.</summary>
        ///
        /// <value>true if the <see cref="T:System.Data.Common.DbDataReader" /> is closed; otherwise false.</value>
        ///
        /// ### <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlDataReader" /> is closed.</exception>
        public override bool IsClosed
        {
            get { return _reader.IsClosed; }
        }

        /// <summary>Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.</summary>
        ///
        /// <value>The number of rows changed, inserted, or deleted. -1 for SELECT statements; 0 if no rows were affected or the statement failed.</value>
        public override int RecordsAffected
        {
            get { return _reader.RecordsAffected; }
        }

        /// <summary>Indexer to get items within this collection using array index syntax.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>The indexed item.</returns>
        public override object this[string name]
        {
            get { return _reader[name]; }
        }

        /// <summary>Indexer to get items within this collection using array index syntax.</summary>
        ///
        /// <param name="ordinal">The ordinal.</param>
        ///
        /// <returns>The indexed item.</returns>
        public override object this[int ordinal]
        {
            get { return _reader[ordinal]; }
        }

        /// <summary>Closes the <see cref="T:System.Data.Common.DbDataReader" /> object.</summary>
        public override void Close()
        {
            // this can occur when we're not profiling, but we've inherited from ProfiledDbCommand and are returning a
            // an unwrapped reader from the base command
            if (_reader != null)
            {
                _reader.Close();
            }

            if (_profiler != null)
            {
                _profiler.ReaderFinish(this);
            }
        }

        /// <summary>Gets the value of the specified column as a Boolean.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override bool GetBoolean(int ordinal)
        {
            return _reader.GetBoolean(ordinal);
        }

        /// <summary>Gets the value of the specified column as a byte.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override byte GetByte(int ordinal)
        {
            return _reader.GetByte(ordinal);
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column, starting at location indicated by <paramref name="dataOffset" />, into the buffer, starting at the location indicated by
        /// <paramref name="bufferOffset" />.
        /// </summary>
        ///
        /// <param name="ordinal">     The zero-based column ordinal.</param>
        /// <param name="dataOffset">  The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">      The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">      The maximum number of characters to read.</param>
        ///
        /// <returns>The actual number of bytes read.</returns>
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return _reader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        /// <summary>Gets the value of the specified column as a single character.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override char GetChar(int ordinal)
        {
            return _reader.GetChar(ordinal);
        }

        /// <summary>
        /// Reads a stream of characters from the specified column, starting at location indicated by <paramref name="dataOffset" />, into the buffer, starting at the location indicated by
        /// <paramref name="bufferOffset" />.
        /// </summary>
        ///
        /// <param name="ordinal">     The zero-based column ordinal.</param>
        /// <param name="dataOffset">  The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">      The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">      The maximum number of characters to read.</param>
        ///
        /// <returns>The actual number of characters read.</returns>
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return _reader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        /// <summary>Gets name of the data type of the specified column.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>A string representing the name of the data type.</returns>
        public override string GetDataTypeName(int ordinal)
        {
            return _reader.GetDataTypeName(ordinal);
        }

        /// <summary>Gets the value of the specified column as a <see cref="T:System.DateTime" /> object.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override DateTime GetDateTime(int ordinal)
        {
            return _reader.GetDateTime(ordinal);
        }

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Decimal" /> object.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override decimal GetDecimal(int ordinal)
        {
            return _reader.GetDecimal(ordinal);
        }

        /// <summary>Gets the value of the specified column as a double-precision floating point number.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override double GetDouble(int ordinal)
        {
            return _reader.GetDouble(ordinal);
        }

        /// <summary>Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</summary>
        ///
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</returns>
        public override System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_reader).GetEnumerator();
        }

        /// <summary>Gets the data type of the specified column.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The data type of the specified column.</returns>
        public override Type GetFieldType(int ordinal)
        {
            return _reader.GetFieldType(ordinal);
        }

        /// <summary>Gets the value of the specified column as a single-precision floating point number.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override float GetFloat(int ordinal)
        {
            return _reader.GetFloat(ordinal);
        }

        /// <summary>Gets the value of the specified column as a globally-unique identifier (GUID).</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override Guid GetGuid(int ordinal)
        {
            return _reader.GetGuid(ordinal);
        }

        /// <summary>Gets the value of the specified column as a 16-bit signed integer.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override short GetInt16(int ordinal)
        {
            return _reader.GetInt16(ordinal);
        }

        /// <summary>Gets the value of the specified column as a 32-bit signed integer.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override int GetInt32(int ordinal)
        {
            return _reader.GetInt32(ordinal);
        }

        /// <summary>Gets the value of the specified column as a 64-bit signed integer.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override long GetInt64(int ordinal)
        {
            return _reader.GetInt64(ordinal);
        }

        /// <summary>Gets the name of the column, given the zero-based column ordinal.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The name of the specified column.</returns>
        public override string GetName(int ordinal)
        {
            return _reader.GetName(ordinal);
        }

        /// <summary>Gets the column ordinal given the name of the column.</summary>
        ///
        /// <param name="name">The name of the column.</param>
        ///
        /// <returns>The zero-based column ordinal.</returns>
        public override int GetOrdinal(string name)
        {
            return _reader.GetOrdinal(name);
        }

        /// <summary>Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.Common.DbDataReader" />.</summary>
        ///
        /// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
        public override DataTable GetSchemaTable()
        {
            return _reader.GetSchemaTable();
        }

        /// <summary>Gets the value of the specified column as an instance of <see cref="T:System.String" />.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override string GetString(int ordinal)
        {
            return _reader.GetString(ordinal);
        }

        /// <summary>Gets the value of the specified column as an instance of <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>The value of the specified column.</returns>
        public override object GetValue(int ordinal)
        {
            return _reader.GetValue(ordinal);
        }

        /// <summary>Populates an array of objects with the column values of the current row.</summary>
        ///
        /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
        ///
        /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
        public override int GetValues(object[] values)
        {
            return _reader.GetValues(values);
        }

        /// <summary>Gets a value that indicates whether the column contains nonexistent or missing values.</summary>
        ///
        /// <param name="ordinal">The zero-based column ordinal.</param>
        ///
        /// <returns>true if the specified column is equivalent to <see cref="T:System.DBNull" />; otherwise false.</returns>
        public override bool IsDBNull(int ordinal)
        {
            return _reader.IsDBNull(ordinal);
        }

        /// <summary>Advances the reader to the next result when reading the results of a batch of statements.</summary>
        ///
        /// <returns>true if there are more result sets; otherwise false.</returns>
        public override bool NextResult()
        {
            return _reader.NextResult();
        }

        /// <summary>Advances the reader to the next record in a result set.</summary>
        ///
        /// <returns>true if there are more rows; otherwise false.</returns>
        public override bool Read()
        {
            return _reader.Read();
        }
    }
}

#pragma warning restore 1591 // xml doc comments warnings