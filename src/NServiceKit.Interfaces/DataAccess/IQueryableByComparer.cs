using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
	public interface IQueryableByComparer
	{
		IList<Extent> Query<Extent>(IComparer<Extent> comparer);
	}
}