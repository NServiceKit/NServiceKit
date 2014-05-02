using Funq;
using NUnit.Framework;
using NServiceKit.Messaging.Tests.Services;

namespace NServiceKit.Messaging.Tests
{
    /// <summary>A messaging host test base.</summary>
	[TestFixture]
	public abstract class MessagingHostTestBase
	{
        /// <summary>Creates message factory.</summary>
        ///
        /// <returns>The new message factory.</returns>
		protected abstract IMessageFactory CreateMessageFactory();

        /// <summary>Creates messaging service.</summary>
        ///
        /// <returns>The new messaging service.</returns>
		protected abstract TransientMessageServiceBase CreateMessagingService();

        /// <summary>Gets or sets the container.</summary>
        ///
        /// <value>The container.</value>
		protected Container Container { get; set; }

        /// <summary>Executes the before each test action.</summary>
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
