using NUnit.Framework;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.WebHost.Endpoints;
using Funq;
using NServiceKit.CacheAccess;
using NServiceKit.CacheAccess.Providers;

namespace NServiceKit.ServiceHost.Tests
{
    /// <summary>A service creation.</summary>
    [Route("/notsingleton")]
    public class ServiceCreation
    {
        /// <summary>Gets or sets a value indicating whether the request filter executed.</summary>
        ///
        /// <value>true if request filter executed, false if not.</value>
        public bool RequestFilterExecuted { get; set; }

        /// <summary>Gets or sets a value indicating whether the contextual request filter executed.</summary>
        ///
        /// <value>true if contextual request filter executed, false if not.</value>
        public bool ContextualRequestFilterExecuted { get; set; }

        /// <summary>Gets or sets a value indicating whether the request filter dependeny is resolved.</summary>
        ///
        /// <value>true if request filter dependeny is resolved, false if not.</value>
        public bool RequestFilterDependenyIsResolved { get; set; }
    }
    /// <summary>A service creation response.</summary>
    public class ServiceCreationResponse
    {
        /// <summary>Gets or sets the number of requests.</summary>
        ///
        /// <value>The number of requests.</value>
        public int RequestCount { get; set; }
    }

    /// <summary>A service creation service.</summary>
    public class ServiceCreationService : ServiceInterface.Service
    {
        /// <summary>The request counter.</summary>
        public int RequestCounter = 0;

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(ServiceCreation request)
        {
            this.RequestCounter++;
            return new ServiceCreationResponse()
            {
                RequestCount = this.RequestCounter
            };
        }
    }

    /// <summary>A service creation test.</summary>
    [TestFixture]
    public class ServiceCreationTest
    {
        private const string ListeningOn = "http://localhost:82/";
        private const string ServiceClientBaseUri = "http://localhost:82/";

        /// <summary>An attribute filters application host HTTP listener.</summary>
        public class AttributeFiltersAppHostHttpListener
            : AppHostHttpListenerBase
        {

            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.ServiceCreationTest.AttributeFiltersAppHostHttpListener class.</summary>
            public AttributeFiltersAppHostHttpListener()
                : base("Service Creation Tests", typeof(ServiceCreationService).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Funq.Container container)
            {
                container.Register<ICacheClient>(new MemoryCacheClient());
            }
        }

        AttributeFiltersAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            EndpointHostConfig.SkipRouteValidation = true;

            appHost = new AttributeFiltersAppHostHttpListener();
            appHost.Init();
            appHost.Start(ListeningOn);
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            appHost.Dispose();
        }

        /// <summary>The rest clients.</summary>
        protected static IRestClient[] RestClients = 
        {
            new JsonServiceClient(ServiceClientBaseUri),
            new XmlServiceClient(ServiceClientBaseUri),
            new JsvServiceClient(ServiceClientBaseUri)
        };

        /// <summary>Service is not singleton.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Service_is_not_singleton(IRestClient client)
        {
            for (int i = 0; i < 5; i++)
            {
                var response = client.Post<ServiceCreationResponse>("notsingleton", new ServiceCreation() { });
                Assert.That(response.RequestCount, Is.EqualTo(1));
            }
        }

        /// <summary>A foo.</summary>
        public class Foo
        {
            /// <summary>Identifier for the global.</summary>
            public static int GlobalId = 0;

            /// <summary>Gets or sets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.ServiceCreationTest.Foo class.</summary>
            public Foo()
            {
                this.Id = GlobalId++;
            }
        }

        /// <summary>Funq is singleton by default.</summary>
        [Test]
        public void Funq_is_singleton_by_Default()
        {
            Foo.GlobalId = 0;
            var container = new Container();
            container.Register(c => new Foo());

            var foo = container.Resolve<Foo>();
            Assert.That(foo.Id, Is.EqualTo(0));
            foo = container.Resolve<Foo>();
            Assert.That(foo.Id, Is.EqualTo(0));
            foo = container.Resolve<Foo>();
            Assert.That(foo.Id, Is.EqualTo(0));
        }

        /// <summary>Funq does transient scope.</summary>
        [Test]
        public void Funq_does_transient_scope()
        {
            Foo.GlobalId = 0;
            var container = new Container();
            container.Register(c => new Foo()).ReusedWithin(ReuseScope.None);

            var foo = container.Resolve<Foo>();
            Assert.That(foo.Id, Is.EqualTo(0));
            foo = container.Resolve<Foo>();
            Assert.That(foo.Id, Is.EqualTo(1));
            foo = container.Resolve<Foo>();
            Assert.That(foo.Id, Is.EqualTo(2));
        }
    }
}
