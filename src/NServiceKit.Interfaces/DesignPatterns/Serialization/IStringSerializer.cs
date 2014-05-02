namespace NServiceKit.DesignPatterns.Serialization
{
    /// <summary>Interface for string serializer.</summary>
	public interface IStringSerializer
	{
        /// <summary>Parses the given from.</summary>
        ///
        /// <typeparam name="TFrom">Type of from.</typeparam>
        /// <param name="from">Source for the.</param>
        ///
        /// <returns>A string.</returns>
		string Parse<TFrom>(TFrom from);
	}
}