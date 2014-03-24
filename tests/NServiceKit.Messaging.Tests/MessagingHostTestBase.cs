using Funq;
using NUnit.Framework;
using NServiceKit.Messaging.Tests.Services;

namespace NServiceKit.Messaging.Tests
{
	[TestFixture]
	public abstract class MessagingHostTestBase
	{
		protected abstract IMessageFactory CreateMessageFactory();

		protected abstract TransientMessageServiceBase CreateMessagingService();

		protected Container Container { get; set; }

		[SetUp]
		public virtual void OnBeforeEachTest()
		{
			if (Container != null)
			{
				Container.Dispose();
			}

			Container = new Container();
			Container.Register(CreateMessageFactory());
		}

	}
}
