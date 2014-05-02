using System;
using System.Collections;
using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
    /// <summary>Interface for basic persistence provider.</summary>
	public interface IBasicPersistenceProvider : IDisposable
	{
        /// <summary>Gets by identifier.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The by identifier.</returns>
		T GetById<T>(object id)
			where T : class, new();

        /// <summary>Gets by identifiers.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="ids">The identifiers.</param>
        ///
        /// <returns>The by identifiers.</returns>
		IList<T> GetByIds<T>(ICollection ids) 
			where T : class, new();

        /// <summary>Stores the given entity.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">The entity.</param>
        ///
        /// <returns>A T.</returns>
		T Store<T>(T entity)
			where T : class, new();

        /// <summary>Stores all.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entities">The entities.</param>
		void StoreAll<TEntity>(IEnumerable<TEntity> entities)
			where TEntity : class, new();

        /// <summary>Deletes the given entity.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">The entity.</param>
		void Delete<T>(T entity)
			where T : class, new();

        /// <summary>Deletes the by identifier described by ID.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="id">The identifier.</param>
		void DeleteById<T>(object id)
			where T : class, new();

        /// <summary>Deletes the by identifiers described by ids.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="ids">The identifiers.</param>
		void DeleteByIds<T>(ICollection ids)
			where T : class, new();

        /// <summary>Deletes all.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
		void DeleteAll<TEntity>()
			where TEntity : class, new();
	}
}