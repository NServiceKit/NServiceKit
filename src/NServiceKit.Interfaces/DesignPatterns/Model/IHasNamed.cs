namespace NServiceKit.DesignPatterns.Model
{
    /// <summary>Interface for has named.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public interface IHasNamed<T>
	{
        /// <summary>Indexer to get or set items within this collection using array index syntax.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        ///
        /// <returns>The indexed item.</returns>
		T this[string listId] { get; set; }
	}
}