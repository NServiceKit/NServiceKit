namespace NServiceKit.Messaging
{
    public interface IMessageHandlerDisposer
    {
        void DisposeMessageHandler(IMessageHandler messageHandler);
    }
}