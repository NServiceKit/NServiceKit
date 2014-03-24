namespace NServiceKit.DataAccess.Criteria
{
	public interface IOrderDescendingCriteria : ICriteria
	{
		string OrderedDescendingBy { get; }
	}
}