using System;
using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
    /// <summary>Interface for queryable by predicate.</summary>
	public interface IQueryableByPredicate
	{
        /// <summary>Queries the given match.</summary>
        ///
        /// <typeparam name="Extent">Type of the extent.</typeparam>
        /// <param name="match">Specifies the match.</param>
        ///
        /// <returns>A list of.</returns>
		IList<Extent> Query<Extent>(Predicate<Extent> match);
	}
}