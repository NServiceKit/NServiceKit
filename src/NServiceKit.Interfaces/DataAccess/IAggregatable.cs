namespace NServiceKit.DataAccess
{
    /// <summary>Interface for aggregatable.</summary>
	public interface IAggregatable
	{
        /// <summary>Finds the avg of the given arguments.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">   The entity.</param>
        /// <param name="fieldName">Name of the field.</param>
        ///
        /// <returns>The calculated average.</returns>
		double GetAvg<T>(T entity, string fieldName);

        /// <summary>Gets a count.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">   The entity.</param>
        /// <param name="fieldName">Name of the field.</param>
        ///
        /// <returns>The count.</returns>
		long GetCount<T>(T entity, string fieldName);

        /// <summary>Finds the min of the given arguments.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">   The entity.</param>
        /// <param name="fieldName">Name of the field.</param>
        ///
        /// <returns>The calculated minimum.</returns>
		T GetMin<T>(T entity, string fieldName);

        /// <summary>Finds the max of the given arguments.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">   The entity.</param>
        /// <param name="fieldName">Name of the field.</param>
        ///
        /// <returns>The calculated maximum.</returns>
		T GetMax<T>(T entity, string fieldName);

        /// <summary>Gets a sum.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">   The entity.</param>
        /// <param name="fieldName">Name of the field.</param>
        ///
        /// <returns>The sum.</returns>
		long GetSum<T>(T entity, string fieldName);
	}
}