#if !SILVERLIGHT 
using System;
using System.Collections;

namespace NServiceKit.Messaging
{
    /// <summary>An in memory transient message service.</summary>
    public class InMemoryTransientMessageService
        : TransientMessageServiceBase
    {
        internal InMemoryTransientMessageFactory Factory { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.InMemoryTransientMessageService class.</summary>
        public InMemoryTransientMessageService()
            : this(null)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.InMemoryTransientMessageService class.</summary>
        ///
        /// <param name="factory">The factory.</param>
        public InMemoryTransientMessageService(InMemoryTransientMessageFactory factory)
        {
            this.Factory = factory ?? new InMemoryTransientMessageFactory(this);
            this.Factory.MqFactory.MessageReceived += factory_MessageReceived;
        }

        void factory_MessageReceived(object sender, EventArgs e)
        {
            //var Factory = (MessageQueueClientFactory) sender;
            this.Start();
        }

        /// <summary>Factory to create consumers and producers that work with this service.</summary>
        ///
        /// <value>The message factory.</value>
        public override IMessageFactory MessageFactory
        {
            get { return Factory; }
        }

        /// <summary>Gets the message queue factory.</summary>
        ///
        /// <value>The message queue factory.</value>
        public MessageQueueClientFactory MessageQueueFactory
        {
            get { return Factory.MqFactory; }
        }
    }
}
#endif