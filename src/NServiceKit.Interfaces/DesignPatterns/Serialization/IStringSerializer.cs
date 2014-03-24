namespace NServiceKit.DesignPatterns.Serialization
{
	public interface IStringSerializer
	{
		string Parse<TFrom>(TFrom from);
	}
}