namespace NServiceKit.DesignPatterns.Model
{
	public interface IHasId<T>
	{
		T Id { get; }
	}
}