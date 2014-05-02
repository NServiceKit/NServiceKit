using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
    /// <summary>Interface for result set.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public interface IResultSet<T>
	{
        /// <summary>Gets the offset.</summary>
        ///
        /// <value>The offset.</value>
		long Offset { get; }

        /// <summary>Gets the number of totals.</summary>
        ///
        /// <value>The total number of count.</value>
		long TotalCount { get; }

        /// <summary>Gets the results.</summary>
        ///
        /// <value>The results.</value>
		IEnumerable<T> Results { get; }
	}
}