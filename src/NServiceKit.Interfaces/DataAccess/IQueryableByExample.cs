using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
    /// <summary>Interface for queryable by example.</summary>
	public interface IQueryableByExample
	{
        /// <summary>Queries by example.</summary>
        ///
        /// <typeparam name="Extent">Type of the extent.</typeparam>
        /// <param name="template">The template.</param>
        ///
        /// <returns>The by example.</returns>
		IList<Extent> QueryByExample<Extent>(object template);
	}
}