using System;

namespace NServiceKit.Messaging
{
    /// <summary>An in memory message queue client.</summary>
    public class InMemoryMessageQueueClient
        : IMessageQueueClient
    {
        private readonly MessageQueueClientFactory factory;

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.InMemoryMessageQueueClient class.</summary>
        ///
        /// <param name="factory">The factory.</param>
        public InMemoryMessageQueueClient(MessageQueueClientFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>Publishes the given message.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="messageBody">The message body.</param>
        public void Publish<T>(T messageBody)
        {
            factory.PublishMessage(QueueNames<T>.In, new Message<T>(messageBody));
        }

        /// <summary>Publishes the given message.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="message">The message.</param>
        public void Publish<T>(IMessage<T> message)
        {
            factory.PublishMessage(QueueNames<T>.In, message);
        }

        /// <summary>Publish the specified message into the durable queue @queueName.</summary>
        ///
        /// <param name="queueName">   .</param>
        /// <param name="messageBytes">.</param>
        public void Publish(string queueName, byte[] messageBytes)
        {
            factory.PublishMessage(queueName, messageBytes);
        }

        /// <summary>Publish the specified message into the transient queue @queueName.</summary>
        ///
        /// <param name="queueName">   .</param>
        /// <param name="messageBytes">.</param>
        public void Notify(string queueName, byte[] messageBytes)
        {
            factory.PublishMessage(queueName, messageBytes);
        }

        /// <summary>Non blocking get message.</summary>
        ///
        /// <param name="queueName">.</param>
        ///
        /// <returns>An array of byte.</returns>
        public byte[] GetAsync(string queueName)
        {
            return factory.GetMessageAsync(queueName);
        }

        /// <summary>Blocking wait for notifications on any of the supplied channels.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="channelNames">.</param>
        ///
        /// <returns>A string.</returns>
        public string WaitForNotifyOnAny(params string[] channelNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>Synchronous blocking get.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="queueName">.</param>
        /// <param name="timeOut">  .</param>
        ///
        /// <returns>A byte[].</returns>
        public byte[] Get(string queueName, TimeSpan? timeOut)
        {
            throw new NotImplementedException();
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
        }
    }
}