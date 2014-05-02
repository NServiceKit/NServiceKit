#if !SILVERLIGHT 
using System;
using NServiceKit.Logging;

namespace NServiceKit.Messaging
{
    /// <summary>An in memory transient message factory.</summary>
    public class InMemoryTransientMessageFactory
        : IMessageFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(InMemoryTransientMessageFactory));
        private readonly InMemoryTransientMessageService  transientMessageService;
        internal MessageQueueClientFactory MqFactory { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.InMemoryTransientMessageFactory class.</summary>
        public InMemoryTransientMessageFactory()
            : this(null)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.InMemoryTransientMessageFactory class.</summary>
        ///
        /// <param name="transientMessageService">The transient message service.</param>
        public InMemoryTransientMessageFactory(InMemoryTransientMessageService transientMessageService)
        {
            this.transientMessageService = transientMessageService ?? new InMemoryTransientMessageService();
            this.MqFactory = new MessageQueueClientFactory();
        }

        /// <summary>Creates message producer.</summary>
        ///
        /// <returns>The new message producer.</returns>
        public IMessageProducer CreateMessageProducer()
        {
            return new InMemoryMessageProducer(this);
        }

        /// <summary>Creates message queue client.</summary>
        ///
        /// <returns>The new message queue client.</returns>
        public IMessageQueueClient CreateMessageQueueClient()
        {
            return new InMemoryMessageQueueClient(MqFactory);
        }

        /// <summary>Creates message service.</summary>
        ///
        /// <returns>The new message service.</returns>
        public IMessageService CreateMessageService()
        {
            return transientMessageService;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Log.DebugFormat("Disposing InMemoryTransientMessageFactory...");
        }


        internal class InMemoryMessageProducer
            : IMessageProducer
        {
            private readonly InMemoryTransientMessageFactory parent;

            /// <summary>Initializes a new instance of the NServiceKit.Messaging.InMemoryTransientMessageFactory.InMemoryMessageProducer class.</summary>
            ///
            /// <param name="parent">The parent.</param>
            public InMemoryMessageProducer(InMemoryTransientMessageFactory parent)
            {
                this.parent = parent;
            }

            /// <summary>Publishes the given message.</summary>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            /// <param name="messageBody">The message body.</param>
            public void Publish<T>(T messageBody)
            {
                Publish((IMessage<T>)new Message<T>(messageBody));
            }

            /// <summary>Publishes the given message.</summary>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            /// <param name="message">The message.</param>
            public void Publish<T>(IMessage<T> message)
            {
                this.parent.transientMessageService.MessageQueueFactory.PublishMessage(QueueNames<T>.In, message);
            }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                Log.DebugFormat("Disposing InMemoryMessageProducer...");
            }
        }

    }
}
#endif