using System.Collections.Generic;
using NServiceKit.DataAccess.Criteria;

namespace NServiceKit.DataAccess
{
    /// <summary>Interface for queryable persistence provider.</summary>
	public interface IQueryablePersistenceProvider : IPersistenceProvider, IQueryable
	{
        /// <summary>Gets all.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="criteria">The criteria.</param>
        ///
        /// <returns>all.</returns>
		IList<T> GetAll<T>(ICriteria criteria)
			where T : class, new();
	}
}