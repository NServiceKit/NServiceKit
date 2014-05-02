using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
    /// <summary>Interface for queryable by comparer.</summary>
	public interface IQueryableByComparer
	{
        /// <summary>Queries the given comparer.</summary>
        ///
        /// <typeparam name="Extent">Type of the extent.</typeparam>
        /// <param name="comparer">The comparer.</param>
        ///
        /// <returns>A list of.</returns>
		IList<Extent> Query<Extent>(IComparer<Extent> comparer);
	}
}