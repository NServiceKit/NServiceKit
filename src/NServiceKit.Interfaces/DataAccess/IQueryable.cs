using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
	public interface IQueryable : IQueryableByExample, IQueryableByPredicate, IQueryableByComparer
	{
		//IList<Extent> Query<Extent>();
		//IList<ElementType> Query<ElementType>(Type extent);
		//IList<Extent> Query<Extent>(Predicate<Extent> match, IComparer<Extent> comparer);
		//IList<Extent> Query<Extent>(Predicate<Extent> match, Comparison<Extent> comparison);
	}
}