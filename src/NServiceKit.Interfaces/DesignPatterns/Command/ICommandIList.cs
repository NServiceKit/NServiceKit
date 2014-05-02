using System.Collections.Generic;

namespace NServiceKit.DesignPatterns.Command
{
    /// <summary>Interface for command i list.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface ICommandIList<T> : IList<T>
    {
    }
}