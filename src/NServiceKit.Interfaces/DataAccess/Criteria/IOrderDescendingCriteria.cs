namespace NServiceKit.DataAccess.Criteria
{
    /// <summary>Interface for order descending criteria.</summary>
	public interface IOrderDescendingCriteria : ICriteria
	{
        /// <summary>Gets who ordered descending this object.</summary>
        ///
        /// <value>Describes who ordered descending this object.</value>
		string OrderedDescendingBy { get; }
	}
}