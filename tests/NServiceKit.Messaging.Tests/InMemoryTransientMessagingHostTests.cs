using System;

namespace NServiceKit.Messaging.Tests
{
    /// <summary>An in memory transient messaging host tests.</summary>
	public class InMemoryTransientMessagingHostTests
		: TransientServiceMessagingTests
	{
		InMemoryTransientMessageService messageService;

        /// <summary>Creates message factory.</summary>
        ///
        /// <returns>The new message factory.</returns>
		protected override IMessageFactory CreateMessageFactory()
		{
			messageService = new InMemoryTransientMessageService();
			return new InMemoryTransientMessageFactory(messageService);
		}

        /// <summary>Creates messaging service.</summary>
        ///
        /// <returns>The new messaging service.</returns>
		protected override TransientMessageServiceBase CreateMessagingService()
		{
			return messageService;
		}
	}
}