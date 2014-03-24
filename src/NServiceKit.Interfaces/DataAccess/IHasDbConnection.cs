#if !SILVERLIGHT && !XBOX
using System.Data;

namespace NServiceKit.DataAccess
{
	public interface IHasDbConnection
	{
		IDbConnection DbConnection { get; }
	}
}
#endif