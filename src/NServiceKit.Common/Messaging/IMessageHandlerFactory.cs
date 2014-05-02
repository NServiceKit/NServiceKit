namespace NServiceKit.Messaging
{
    /// <summary>
    /// Encapsulates creating a new message handler
    /// </summary>
    public interface IMessageHandlerFactory
    {
        /// <summary>Handler, called when the create message.</summary>
        ///
        /// <returns>The new message handler.</returns>
        IMessageHandler CreateMessageHandler();
    }
}