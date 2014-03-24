using System.Collections.Generic;

namespace NServiceKit.DesignPatterns.Command
{
    public interface ICommandIEnumerable<T> : ICommand<IEnumerable<T>>
    {
    }
}