using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
	public interface IQueryableByExample
	{
		IList<Extent> QueryByExample<Extent>(object template);
	}
}