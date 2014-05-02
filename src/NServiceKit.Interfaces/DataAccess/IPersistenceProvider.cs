using System;
using System.Collections;
using System.Collections.Generic;

namespace NServiceKit.DataAccess
{
    /// <summary>Interface for persistence provider.</summary>
	public interface IPersistenceProvider : IBasicPersistenceProvider, IDisposable
	{
        /// <summary>Gets all.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>all.</returns>
		IList<T> GetAll<T>()
			where T : class, new();

        /// <summary>Gets all ordered by.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="sortAsc">  true to sort ascending.</param>
        ///
        /// <returns>all ordered by.</returns>
		IList<T> GetAllOrderedBy<T>(string fieldName, bool sortAsc)
			where T : class, new();

        /// <summary>Searches for the first value.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The found value.</returns>
		T FindByValue<T>(string name, object value)
			where T : class, new();

        /// <summary>Searches for all by value.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The found all by value.</returns>
		IList<T> FindAllByValue<T>(string name, object value)
			where T : class, new();

        /// <summary>Searches for the first values.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="name">  The name.</param>
        /// <param name="values">The values.</param>
        ///
        /// <returns>The found values.</returns>
		IList<T> FindByValues<T>(string name, ICollection values)
			where T : class, new();

        /// <summary>Flushes this object.</summary>
		void Flush();

        /// <summary>Stores all.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entities">The entities.</param>
        ///
        /// <returns>A list of.</returns>
		IList<T> StoreAll<T>(IList<T> entities)
			where T : class, new();

        /// <summary>Deletes all described by entities.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entities">The entities.</param>
		void DeleteAll<T>(IList<T> entities)
			where T : class, new();

        /// <summary>Begins a transaction.</summary>
        ///
        /// <returns>An ITransactionContext.</returns>
		ITransactionContext BeginTransaction();
	}
}