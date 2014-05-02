using System.IO;
using Funq;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.MiniProfiler.UI;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A by identifier.</summary>
    [Route("/request/{Id}")]
    public class ById
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public string Id { get; set; }
    }

    /// <summary>A unique request service.</summary>
    public class UniqueRequestService : IService
    {
        /// <summary>Gets the given by identifier.</summary>
        ///
        /// <param name="byId">The by identifier to get.</param>
        ///
        /// <returns>A string.</returns>
        public string Get(ById byId)
        {
            return byId.Id;
        }
    }

    /// <summary>A unique request application host.</summary>
    public class UniqueRequestAppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.UniqueRequestAppHost class.</summary>
        public UniqueRequestAppHost() : base("Unique Request Tests", typeof(UniqueRequestService).Assembly) {}

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container) {}
    }

    /// <summary>A unique request tests.</summary>
    [TestFixture]
    public class UniqueRequestTests
    {
        private const string BaseUri = "http://localhost:8000";
        private UniqueRequestAppHost appHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new UniqueRequestAppHost();
            appHost.Init();
            appHost.Start(BaseUri + "/");
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        /// <summary>Can handle encoded characters.</summary>
        [Test]
        public void Can_handle_encoded_chars()
        {
            var response = BaseUri.CombineWith("request/123%20456").GetStringFromUrl();
            Assert.That(response, Is.EqualTo("123 456"));
            response = BaseUri.CombineWith("request/123%7C456").GetStringFromUrl();
            Assert.That(response, Is.EqualTo("123|456"));
        }
    }
}
