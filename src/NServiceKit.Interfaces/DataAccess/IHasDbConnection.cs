#if !SILVERLIGHT && !XBOX
using System.Data;

namespace NServiceKit.DataAccess
{
    /// <summary>Interface for has database connection.</summary>
	public interface IHasDbConnection
	{
        /// <summary>Gets the database connection.</summary>
        ///
        /// <value>The database connection.</value>
		IDbConnection DbConnection { get; }
	}
}
#endif