using System.Collections.Generic;

namespace NServiceKit.DesignPatterns.Command
{
    /// <summary>Interface for command i enumerable.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface ICommandIEnumerable<T> : ICommand<IEnumerable<T>>
    {
    }
}