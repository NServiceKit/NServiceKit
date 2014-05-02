using System;
using System.Collections;
using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
	/// <summary>
	/// For providers that want a cleaner API with a little more perf
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IBasicPersistenceProvider<T> 
		: IDisposable
	{
        /// <summary>Gets by identifier.</summary>
        ///
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The by identifier.</returns>
		T GetById(object id);

        /// <summary>Gets by identifiers.</summary>
        ///
        /// <param name="ids">The identifiers.</param>
        ///
        /// <returns>The by identifiers.</returns>
		IList<T> GetByIds(IEnumerable ids);

        /// <summary>Gets all.</summary>
        ///
        /// <returns>all.</returns>
		IList<T> GetAll();

        /// <summary>Stores the given entity.</summary>
        ///
        /// <param name="entity">The entity to delete.</param>
        ///
        /// <returns>A T.</returns>
		T Store(T entity);

        /// <summary>Stores all.</summary>
        ///
        /// <param name="entities">The entities.</param>
		void StoreAll(IEnumerable<T> entities);

        /// <summary>Deletes the given entity.</summary>
        ///
        /// <param name="entity">The entity to delete.</param>
		void Delete(T entity);

        /// <summary>Deletes the by identifier described by ID.</summary>
        ///
        /// <param name="id">The identifier.</param>
		void DeleteById(object id);

        /// <summary>Deletes the by identifiers described by ids.</summary>
        ///
        /// <param name="ids">The identifiers.</param>
		void DeleteByIds(IEnumerable ids);

        /// <summary>Deletes all.</summary>
		void DeleteAll();
	}
}