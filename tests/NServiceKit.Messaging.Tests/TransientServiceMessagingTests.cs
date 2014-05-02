using Funq;
using NUnit.Framework;
using NServiceKit.Messaging.Tests.Services;

namespace NServiceKit.Messaging.Tests
{
    /// <summary>A transient service messaging tests.</summary>
	public abstract class TransientServiceMessagingTests
		: MessagingHostTestBase
	{
        /// <summary>Executes the before each test action.</summary>
		public override void OnBeforeEachTest()
		{
			base.OnBeforeEachTest();
            // TODO: Reevaluate this entire pattern
		    Container.Register(new GreetService());
            Container.Register(new AlwaysFailService());
            Container.Register(new UnRetryableFailService());
		}

        /// <summary>Normal greet service client and server example.</summary>
		[Test]
		public void Normal_GreetService_client_and_server_example()
		{
			var service = Container.Resolve<GreetService>();
			using (var serviceHost = CreateMessagingService())
			{
				serviceHost.RegisterHandler<Greet>(service.ExecuteAsync);

				serviceHost.Start();

				using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
				{
					client.Publish(new Greet { Name = "World!" });
				}

				Assert.That(service.Result, Is.EqualTo("Hello, World!"));
				Assert.That(service.TimesCalled, Is.EqualTo(1));
			}
		}

        /// <summary>Publish before starting host greet service client and server example.</summary>
		[Test]
		public void Publish_before_starting_host_GreetService_client_and_server_example()
		{
			var service = Container.Resolve<GreetService>();
			using (var serviceHost = CreateMessagingService())
			{
				using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
				{
					client.Publish(new Greet { Name = "World!" });
				}

				serviceHost.RegisterHandler<Greet>(service.ExecuteAsync);
				serviceHost.Start();

				Assert.That(service.Result, Is.EqualTo("Hello, World!"));
				Assert.That(service.TimesCalled, Is.EqualTo(1));
			}
		}

        /// <summary>Always fails service ends up in dlq after 3 attempts.</summary>
		[Test]
		public void AlwaysFailsService_ends_up_in_dlq_after_3_attempts()
		{
			var service = Container.Resolve<AlwaysFailService>();
			var request = new AlwaysFail { Name = "World!" };
			using (var serviceHost = CreateMessagingService())
			{
				using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
				{
					client.Publish(request);
				}

				serviceHost.RegisterHandler<AlwaysFail>(service.ExecuteAsync);
				serviceHost.Start();

				Assert.That(service.Result, Is.Null);
				Assert.That(service.TimesCalled, Is.EqualTo(3));

				using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
				{
					var dlqMessage = client.GetAsync(QueueNames<AlwaysFail>.Dlq)
						.ToMessage<AlwaysFail>();

					Assert.That(dlqMessage, Is.Not.Null);
					Assert.That(dlqMessage.GetBody().Name, Is.EqualTo(request.Name));
				}
			}
		}

        /// <summary>Un retryable fail service ends up in dlq after 1 attempt.</summary>
		[Test]
		public void UnRetryableFailService_ends_up_in_dlq_after_1_attempt()
		{
			var service = Container.Resolve<UnRetryableFailService>();
			var request = new UnRetryableFail { Name = "World!" };
			using (var serviceHost = CreateMessagingService())
			{
				using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
				{
					client.Publish(request);
				}

				serviceHost.RegisterHandler<UnRetryableFail>(service.ExecuteAsync);
				serviceHost.Start();

				Assert.That(service.Result, Is.Null);
				Assert.That(service.TimesCalled, Is.EqualTo(1));

				using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
				{
					var dlqMessage = client.GetAsync(QueueNames<UnRetryableFail>.Dlq)
						.ToMessage<UnRetryableFail>();

					Assert.That(dlqMessage, Is.Not.Null);
					Assert.That(dlqMessage.GetBody().Name, Is.EqualTo(request.Name));
				}
			}
		}

	}
}