namespace NServiceKit.DataAccess.Criteria
{
    /// <summary>Interface for order ascending criteria.</summary>
	public interface IOrderAscendingCriteria : ICriteria
	{
        /// <summary>Gets who ordered ascending this object.</summary>
        ///
        /// <value>Describes who ordered ascending this object.</value>
		string OrderedAscendingBy { get; }
	}
}