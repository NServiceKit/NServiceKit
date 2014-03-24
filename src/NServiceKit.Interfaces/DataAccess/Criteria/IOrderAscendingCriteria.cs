namespace NServiceKit.DataAccess.Criteria
{
	public interface IOrderAscendingCriteria : ICriteria
	{
		string OrderedAscendingBy { get; }
	}
}