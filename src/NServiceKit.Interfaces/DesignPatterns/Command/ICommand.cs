namespace NServiceKit.DesignPatterns.Command
{
    /// <summary>Interface for command.</summary>
    ///
    /// <typeparam name="ReturnType">Type of the return type.</typeparam>
    public interface ICommand<ReturnType>
    {
        /// <summary>Gets the execute.</summary>
        ///
        /// <returns>A ReturnType.</returns>
        ReturnType Execute();
    }
}