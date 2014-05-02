namespace NServiceKit.DataAccess.Criteria
{
    /// <summary>Interface for paging criteria.</summary>
	public interface IPagingCriteria : ICriteria
	{
        /// <summary>Gets the result offset.</summary>
        ///
        /// <value>The result offset.</value>
		uint ResultOffset { get; }

        /// <summary>Gets the result limit.</summary>
        ///
        /// <value>The result limit.</value>
		uint ResultLimit { get; }
	}
}