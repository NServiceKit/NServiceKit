using System;

namespace NServiceKit.Messaging
{
	public interface IMessageFactory : IMessageQueueClientFactory
	{
		IMessageProducer CreateMessageProducer();
	}
}