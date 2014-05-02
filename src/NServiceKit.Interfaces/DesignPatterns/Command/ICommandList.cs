using System.Collections.Generic;

namespace NServiceKit.DesignPatterns.Command
{
    /// <summary>Interface for command list.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface ICommandList<T> : ICommand<List<T>>
    {
    }
}