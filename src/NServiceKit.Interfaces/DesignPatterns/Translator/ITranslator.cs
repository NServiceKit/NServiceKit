namespace NServiceKit.DesignPatterns.Translator
{
    /// <summary>Interface for translator.</summary>
    ///
    /// <typeparam name="To">  Type of to.</typeparam>
    /// <typeparam name="From">Type of from.</typeparam>
    public interface ITranslator<To, From>
    {
        /// <summary>Parses the given from.</summary>
        ///
        /// <param name="from">Source for the.</param>
        ///
        /// <returns>To.</returns>
        To Parse(From from);
    }
}