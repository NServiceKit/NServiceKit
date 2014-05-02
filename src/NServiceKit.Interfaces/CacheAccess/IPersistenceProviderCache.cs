using System;
using System.Collections;
using System.Collections.Generic;

namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for persistence provider cache.</summary>
	public interface IPersistenceProviderCache
	{
        /// <summary>Gets by identifier.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entityId">Identifier for the entity.</param>
        ///
        /// <returns>The by identifier.</returns>
		TEntity GetById<TEntity>(object entityId)
			where TEntity : class, new();

        /// <summary>Gets by identifiers.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entityIds">List of identifiers for the entities.</param>
        ///
        /// <returns>The by identifiers.</returns>
		List<TEntity> GetByIds<TEntity>(ICollection entityIds)
			where TEntity : class, new();

        /// <summary>Sets a cache.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
		void SetCache<TEntity>(TEntity entity)
			where TEntity : class, new();

        /// <summary>Stores the given entity.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
		void Store<TEntity>(TEntity entity)
			where TEntity : class, new();

        /// <summary>Stores all.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entities">A variable-length parameters list containing entities.</param>
		void StoreAll<TEntity>(params TEntity[] entities)
			where TEntity : class, new();

        /// <summary>Clears all described by entityIds.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entityIds">List of identifiers for the entities.</param>
		void ClearAll<TEntity>(ICollection entityIds)
			where TEntity : class, new();

        /// <summary>Clears this object to its blank/initial state.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entityIds">List of identifiers for the entities.</param>
		void Clear<TEntity>(params object[] entityIds)
			where TEntity : class, new();
	}
}