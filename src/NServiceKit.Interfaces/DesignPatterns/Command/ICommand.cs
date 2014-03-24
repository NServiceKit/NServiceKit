namespace NServiceKit.DesignPatterns.Command
{
    public interface ICommand<ReturnType>
    {
        ReturnType Execute();
    }
}