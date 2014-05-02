using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Service;
using NServiceKit.ServiceInterface;
using NServiceKit.CacheAccess;
using NServiceKit.CacheAccess.Providers;
using NServiceKit.ServiceInterface.Cors;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Utils;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>The cors feature request.</summary>
    [Route("/corsmethod")]
    public class CorsFeatureRequest
    {
    }

    [EnableCors("http://localhost http://localhost2", "POST, GET", "Type1, Type2", true)]
    public class CorsFeatureResponse
    {
        /// <summary>Gets or sets a value indicating whether this object is success.</summary>
        ///
        /// <value>true if this object is success, false if not.</value>
        public bool IsSuccess { get; set; }
    }

    /// <summary>The cors feature service.</summary>
    public class CorsFeatureService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A CorsFeatureResponse.</returns>
        public CorsFeatureResponse Any(CorsFeatureRequest request)
        {
            return new CorsFeatureResponse { IsSuccess = true };
        }
    }

    /// <summary>A global cors feature request.</summary>
    [Route("/globalcorsfeature")]
    public class GlobalCorsFeatureRequest
    {
    }

    /// <summary>A global cors feature response.</summary>
    public class GlobalCorsFeatureResponse
    {
        /// <summary>Gets or sets a value indicating whether this object is success.</summary>
        ///
        /// <value>true if this object is success, false if not.</value>
        public bool IsSuccess { get; set; }
    }

    /// <summary>A global cors feature service.</summary>
    public class GlobalCorsFeatureService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A GlobalCorsFeatureResponse.</returns>
        public GlobalCorsFeatureResponse Any(GlobalCorsFeatureRequest request)
        {
            return new GlobalCorsFeatureResponse { IsSuccess = true };
        }
    }

    /// <summary>The cors feature service test.</summary>
    [TestFixture]
    public class CorsFeatureServiceTest
    {
        private const string ListeningOn = "http://localhost:8022/";
        private const string ServiceClientBaseUri = "http://localhost:8022/";

        /// <summary>The cors feature application host HTTP listener.</summary>
        public class CorsFeatureAppHostHttpListener
            : AppHostHttpListenerBase
        {

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.CorsFeatureServiceTest.CorsFeatureAppHostHttpListener class.</summary>
            public CorsFeatureAppHostHttpListener()
                : base("Cors Feature Tests", typeof(CorsFeatureService).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Funq.Container container)
            {
            }
        }

        CorsFeatureAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            appHost = new CorsFeatureAppHostHttpListener();
            appHost.Init();
            appHost.Start(ListeningOn);
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            appHost.Dispose();
        }

        static IRestClient[] RestClients = 
        {
            new JsonServiceClient(ServiceClientBaseUri),
            new XmlServiceClient(ServiceClientBaseUri),
            new JsvServiceClient(ServiceClientBaseUri)
        };

        /// <summary>Executes for 5 mins operation.</summary>
        [Test, Explicit]
        public void RunFor5Mins()
        {
            Thread.Sleep(TimeSpan.FromMinutes(5));
        }

        /// <summary>Cors method has access control headers.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void CorsMethodHasAccessControlHeaders(IRestClient client)
        {
            appHost.Config.GlobalResponseHeaders.Clear();

            var response = RequestContextTests.GetResponseHeaders(ListeningOn + "/corsmethod");
            Assert.That(response[HttpHeaders.AllowOrigin], Is.EqualTo("http://localhost http://localhost2"));
            Assert.That(response[HttpHeaders.AllowMethods], Is.EqualTo("POST, GET"));
            Assert.That(response[HttpHeaders.AllowHeaders], Is.EqualTo("Type1, Type2"));
            Assert.That(response[HttpHeaders.AllowCredentials], Is.EqualTo("true"));
        }

        /// <summary>Global cors has access control headers.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void GlobalCorsHasAccessControlHeaders(IRestClient client)
        {
            appHost.LoadPlugin(new CorsFeature());

            var response = RequestContextTests.GetResponseHeaders(ListeningOn + "/globalcorsfeature");
            Assert.That(response[HttpHeaders.AllowOrigin], Is.EqualTo("*"));
            Assert.That(response[HttpHeaders.AllowMethods], Is.EqualTo("GET, POST, PUT, DELETE, OPTIONS"));
            Assert.False(response.ContainsKey(HttpHeaders.AllowCredentials));
            Assert.That(response[HttpHeaders.AllowHeaders], Is.EqualTo("Content-Type"));
        }

    }
}
