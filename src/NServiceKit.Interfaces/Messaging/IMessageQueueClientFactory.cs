using System;

namespace NServiceKit.Messaging
{
	public interface IMessageQueueClientFactory
		: IDisposable
	{
		IMessageQueueClient CreateMessageQueueClient();
	}
}