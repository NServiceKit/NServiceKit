using System;

namespace NServiceKit.Messaging
{
    /// <summary>Interface for message queue client factory.</summary>
	public interface IMessageQueueClientFactory
		: IDisposable
	{
        /// <summary>Creates message queue client.</summary>
        ///
        /// <returns>The new message queue client.</returns>
		IMessageQueueClient CreateMessageQueueClient();
	}
}