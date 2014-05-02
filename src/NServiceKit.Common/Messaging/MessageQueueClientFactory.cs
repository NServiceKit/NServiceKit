using System;
using System.Collections.Generic;

namespace NServiceKit.Messaging
{
    /// <summary>A message queue client factory.</summary>
    public class MessageQueueClientFactory
        : IMessageQueueClientFactory
    {
        /// <summary>Creates message queue client.</summary>
        ///
        /// <returns>The new message queue client.</returns>
        public IMessageQueueClient CreateMessageQueueClient()
        {
            return new InMemoryMessageQueueClient(this);
        }

        readonly object syncLock = new object();

        /// <summary>Occurs when Message Received.</summary>
        public event EventHandler<EventArgs> MessageReceived;

        void InvokeMessageReceived(EventArgs e)
        {
            var received = MessageReceived;
            if (received != null) received(this, e);
        }

        private readonly Dictionary<string, Queue<byte[]>> queueMessageBytesMap
            = new Dictionary<string, Queue<byte[]>>();

        /// <summary>Publish message.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="queueName">.</param>
        /// <param name="message">  The message.</param>
        public void PublishMessage<T>(string queueName, IMessage<T> message)
        {
            PublishMessage(queueName, message.ToBytes());
        }

        /// <summary>Publish message.</summary>
        ///
        /// <param name="queueName">   .</param>
        /// <param name="messageBytes">The message in bytes.</param>
        public void PublishMessage(string queueName, byte[] messageBytes)
        {
            lock (syncLock)
            {
                Queue<byte[]> bytesQueue;
                if (!queueMessageBytesMap.TryGetValue(queueName, out bytesQueue))
                {
                    bytesQueue = new Queue<byte[]>();
                    queueMessageBytesMap[queueName] = bytesQueue;
                }

                bytesQueue.Enqueue(messageBytes);
            }

            InvokeMessageReceived(new EventArgs());
        }

        /// <summary>
        /// Returns the next message from queueName or null if no message
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public byte[] GetMessageAsync(string queueName)
        {
            lock (syncLock)
            {
                Queue<byte[]> bytesQueue;
                if (!queueMessageBytesMap.TryGetValue(queueName, out bytesQueue))
                {
                    return null;
                }

                if (bytesQueue.Count == 0)
                {
                    return null;
                }

                var messageBytes = bytesQueue.Dequeue();
                return messageBytes;
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
        }
    }
}