using System;

namespace NServiceKit.Messaging
{
    /// <summary>Interface for message factory.</summary>
	public interface IMessageFactory : IMessageQueueClientFactory
	{
        /// <summary>Creates message producer.</summary>
        ///
        /// <returns>The new message producer.</returns>
		IMessageProducer CreateMessageProducer();
	}
}