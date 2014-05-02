using Funq;
using NUnit.Framework;
using NServiceKit.Configuration;
using NServiceKit.Messaging;
using NServiceKit.Redis;
using NServiceKit.Redis.Messaging;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Common.Tests.Messaging
{
    /// <summary>any test mq.</summary>
    public class AnyTestMq
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }

    /// <summary>any test mq response.</summary>
    public class AnyTestMqResponse
    {
        /// <summary>Gets or sets the identifier of the correlation.</summary>
        ///
        /// <value>The identifier of the correlation.</value>
        public int CorrelationId { get; set; }
    }

    /// <summary>A post test mq.</summary>
    public class PostTestMq
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }

    /// <summary>A post test mq response.</summary>
    public class PostTestMqResponse
    {
        /// <summary>Gets or sets the identifier of the correlation.</summary>
        ///
        /// <value>The identifier of the correlation.</value>
        public int CorrelationId { get; set; }
    }

    /// <summary>A test mq service.</summary>
    public class TestMqService : IService
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(AnyTestMq request)
        {
            return new AnyTestMqResponse { CorrelationId = request.Id };
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(PostTestMq request)
        {
            return new PostTestMqResponse { CorrelationId = request.Id };
        }
    }

    /// <summary>An application host.</summary>
    public class AppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Messaging.AppHost class.</summary>
        public AppHost()
            : base("Service Name", typeof(AnyTestMq).Assembly) { }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container)
        {
            var appSettings = new AppSettings();
            container.Register<IRedisClientsManager>(c => new PooledRedisClientManager(
                new string[] { appSettings.GetString("Redis.Host") ?? "localhost" }));
            container.Register<IMessageService>(c => new RedisMqServer(c.Resolve<IRedisClientsManager>()));
            container.Register<IMessageFactory>(c => c.Resolve<IMessageService>().MessageFactory);

            var mqServer = (RedisMqServer)container.Resolve<IMessageService>();
            mqServer.RegisterHandler<AnyTestMq>(ServiceController.ExecuteMessage);
            mqServer.RegisterHandler<PostTestMq>(ServiceController.ExecuteMessage);

            mqServer.Start();
        }
    }

    /// <summary>The redis mq server tests.</summary>
    [TestFixture]
    [Ignore]
    public class RedisMqServerTests
    {
        private const string ListeningOn = "http://*:1337/";
        /// <summary>The host.</summary>
        public const string Host = "http://localhost:1337";
        private const string BaseUri = Host + "/";

        AppHost appHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new AppHost();
            appHost.Init();
            appHost.Start(ListeningOn);

            using (var redis = appHost.TryResolve<IRedisClientsManager>().GetClient())
                redis.FlushAll();
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        /// <summary>Can publish to any test mq service.</summary>
        [Test]
        public void Can_Publish_to_AnyTestMq_Service()
        {
            using (var mqFactory = appHost.TryResolve<IMessageFactory>())
            {
                var request = new AnyTestMq { Id = 1 };
                mqFactory.CreateMessageProducer().Publish(request);
                var msg = mqFactory.CreateMessageQueueClient().Get(QueueNames<AnyTestMqResponse>.In, null)
                    .ToMessage<AnyTestMqResponse>();
                Assert.That(msg.GetBody().CorrelationId, Is.EqualTo(request.Id));
            }
        }

        /// <summary>Can publish to post test mq service.</summary>
        [Test]
        public void Can_Publish_to_PostTestMq_Service()
        {
            using (var mqFactory = appHost.TryResolve<IMessageFactory>())
            {
                var request = new PostTestMq { Id = 2 };
                mqFactory.CreateMessageProducer().Publish(request);
                var msg = mqFactory.CreateMessageQueueClient().Get(QueueNames<PostTestMqResponse>.In, null)
                    .ToMessage<PostTestMqResponse>();
                Assert.That(msg.GetBody().CorrelationId, Is.EqualTo(request.Id));
            }
        }

        /// <summary>Sends the one way calls any test mq service via mq.</summary>
        [Test]
        public void SendOneWay_calls_AnyTestMq_Service_via_MQ()
        {
            var client = new JsonServiceClient(BaseUri);
            var request = new AnyTestMq { Id = 3 };

            client.SendOneWay(request);

            using (var mqFactory = appHost.TryResolve<IMessageFactory>())
            {
                var msg = mqFactory.CreateMessageQueueClient().Get(QueueNames<AnyTestMqResponse>.In, null)
                    .ToMessage<AnyTestMqResponse>();
                Assert.That(msg.GetBody().CorrelationId, Is.EqualTo(request.Id));
            }
        }

        /// <summary>Sends the one way calls post test mq service via mq.</summary>
        [Test]
        public void SendOneWay_calls_PostTestMq_Service_via_MQ()
        {
            var client = new JsonServiceClient(BaseUri);
            var request = new PostTestMq { Id = 4 };

            client.SendOneWay(request);

            using (var mqFactory = appHost.TryResolve<IMessageFactory>())
            {
                var msg = mqFactory.CreateMessageQueueClient().Get(QueueNames<PostTestMqResponse>.In, null)
                    .ToMessage<PostTestMqResponse>();
                Assert.That(msg.GetBody().CorrelationId, Is.EqualTo(request.Id));
            }
        }
    }
}