namespace NServiceKit.DesignPatterns.Model
{
    /// <summary>Interface for has identifier.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public interface IHasId<T>
	{
        /// <summary>Gets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		T Id { get; }
	}
}