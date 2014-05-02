namespace NServiceKit.Messaging
{
    /// <summary>Interface for message handler disposer.</summary>
    public interface IMessageHandlerDisposer
    {
        /// <summary>Handler, called when the dispose message.</summary>
        ///
        /// <param name="messageHandler">The message handler.</param>
        void DisposeMessageHandler(IMessageHandler messageHandler);
    }
}