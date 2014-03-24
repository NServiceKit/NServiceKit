using System;
using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
	public interface IQueryableByPredicate
	{
		IList<Extent> Query<Extent>(Predicate<Extent> match);
	}
}