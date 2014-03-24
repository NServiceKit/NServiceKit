using System.Collections.Generic;

namespace NServiceKit.DesignPatterns.Command
{
    public interface ICommandList<T> : ICommand<List<T>>
    {
    }
}