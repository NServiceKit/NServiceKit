using Funq;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>An application host listener echo request tests.</summary>
    [TestFixture]
    public class AppHostListenerEchoRequestTests
    {
        /// <summary>An application host.</summary>
        public class AppHost : AppHostHttpListenerBase
        {
            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.AppHostListenerEchoRequestTests.AppHost class.</summary>
            public AppHost()
                : base("Echo AppHost", typeof(AppHost).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Container container) {}
        }

        /// <summary>An echo.</summary>
        [Route("/echo")]
        [Route("/echo/{PathInfoParam}")]
        public class Echo : IReturn<Echo>
        {
            /// <summary>Gets or sets the parameter.</summary>
            ///
            /// <value>The parameter.</value>
            public string Param { get; set; }

            /// <summary>Gets or sets the path information parameter.</summary>
            ///
            /// <value>The path information parameter.</value>
            public string PathInfoParam { get; set; }
        }

        /// <summary>An echo service.</summary>
        public class EchoService : ServiceInterface.Service
        {
            /// <summary>Anies the given request.</summary>
            ///
            /// <param name="request">The request.</param>
            ///
            /// <returns>A RequestInfoResponse.</returns>
            public Echo Any(Echo request)
            {
                return request;
            }

            /// <summary>Anies the given request.</summary>
            ///
            /// <param name="request">The request.</param>
            ///
            /// <returns>A RequestInfoResponse.</returns>
            public RequestInfoResponse Any(RequestInfo request)
            {
                var requestInfo = RequestInfoHandler.GetRequestInfo(base.Request);
                return requestInfo;
            }
        }

        private AppHost appHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new AppHost();
            appHost.Init();
            appHost.Start(Config.AbsoluteBaseUri);
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
        }

        /// <summary>Does URL decode raw query string.</summary>
        [Test]
        public void Does_url_decode_raw_QueryString()
        {
            var testEncoding = "test://?&% encoding";
            var url = "{0}echo?Param={1}".Fmt(Config.AbsoluteBaseUri, testEncoding.UrlEncode());
            Assert.That(url, Is.StringEnding("/echo?Param=test%3a%2f%2f%3f%26%25%20encoding"));

            var json = url.GetJsonFromUrl();
            var response = json.FromJson<Echo>();
            Assert.That(response.Param, Is.EqualTo(testEncoding));
        }

        /// <summary>Does URL decode raw path information.</summary>
        [Test]
        public void Does_url_decode_raw_PathInfo()
        {
            var testEncoding = "test encoding";
            var url = "{0}echo/{1}".Fmt(Config.AbsoluteBaseUri, testEncoding.UrlEncode());
            Assert.That(url, Is.StringEnding("/echo/test%20encoding"));

            var json = url.GetJsonFromUrl();
            var response = json.FromJson<Echo>();
            Assert.That(response.PathInfoParam, Is.EqualTo(testEncoding));
        }

        /// <summary>Does URL transparently decode query string.</summary>
        [Test]
        public void Does_url_transparently_decode_QueryString()
        {
            var client = new JsonServiceClient(Config.AbsoluteBaseUri);
            var request = new Echo { Param = "test://?&% encoding" };
            var response = client.Get(request);
            Assert.That(response.Param, Is.EqualTo(request.Param));
        }

        /// <summary>Does URL transparently decode path information.</summary>
        [Test]
        public void Does_url_transparently_decode_PathInfo()
        {
            var client = new JsonServiceClient(Config.AbsoluteBaseUri);
            var request = new Echo { PathInfoParam = "test:?&% encoding" };
            var response = client.Get(request);
            Assert.That(response.Param, Is.EqualTo(request.Param));
        }

        /// <summary>Does URL transparently decode request body.</summary>
        [Test]
        public void Does_url_transparently_decode_RequestBody()
        {
            var client = new JsonServiceClient(Config.AbsoluteBaseUri);
            var request = new Echo { Param = "test://?&% encoding" };
            var response = client.Post(request);
            Assert.That(response.Param, Is.EqualTo(request.Param));
        }

    }
}