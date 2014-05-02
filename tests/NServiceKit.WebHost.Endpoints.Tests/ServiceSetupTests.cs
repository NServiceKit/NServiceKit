using Funq;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A service setup.</summary>
    public class ServiceSetup : IReturn<ServiceSetup>
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }

    /// <summary>A base service.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class BaseService<T> : ServiceInterface.Service
    {
        /// <summary>Gets the given dto.</summary>
        ///
        /// <param name="dto">The dto to get.</param>
        ///
        /// <returns>An object.</returns>
        public virtual object Get(T dto)
        {
            return null;
        }
    }

    /// <summary>An actual.</summary>
    public class Actual : BaseService<ServiceSetup>
    {
        /// <summary>Gets the given dto.</summary>
        ///
        /// <param name="dto">The dto to get.</param>
        ///
        /// <returns>An object.</returns>
        public override object Get(ServiceSetup dto)
        {
            dto.Id++;
            return dto;
        }
    }

    /// <summary>A service setup application host.</summary>
    public class ServiceSetupAppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.ServiceSetupAppHost class.</summary>
        public ServiceSetupAppHost() : base("Service Setup Tests", typeof(Actual).Assembly) { }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container) { }
    }

    /// <summary>A service setup tests.</summary>
    [TestFixture]
    public class ServiceSetupTests
    {
        private const string BaseUri = "http://localhost:8000/";
        JsonServiceClient client = new JsonServiceClient(BaseUri);
        private ServiceSetupAppHost appHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new ServiceSetupAppHost();
            appHost.Init();
            appHost.Start(BaseUri);
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        /// <summary>Can still load with abstract generic base types.</summary>
        [Test]
        public void Can_still_load_with_Abstract_Generic_BaseTypes()
        {
            var response = client.Get(new ServiceSetup { Id = 1 });
            Assert.That(response.Id, Is.EqualTo(2));
        }
    }
}