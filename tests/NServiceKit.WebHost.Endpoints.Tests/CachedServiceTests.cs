using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Plugins.ProtoBuf;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A cached service tests.</summary>
	[TestFixture]
	public class CachedServiceTests
	{
        ExampleAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            appHost = new ExampleAppHostHttpListener();
            appHost.Init();
            appHost.Start(Config.AbsoluteBaseUri);
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            appHost.Dispose();
        }

        /// <summary>Can call cached web service with JSON.</summary>
        [Test]
        public void Can_call_Cached_WebService_with_JSON()
        {
            var client = new JsonServiceClient(Config.NServiceKitBaseUri);

            var response = client.Get<MoviesResponse>("/cached/movies");

            Assert.That(response.Movies.Count, Is.EqualTo(ResetMoviesService.Top5Movies.Count));
        }

        /// <summary>Can call cached web service with prototype buffer.</summary>
        [Test]
        public void Can_call_Cached_WebService_with_ProtoBuf()
        {
            var client = new ProtoBufServiceClient(Config.NServiceKitBaseUri);

            var response = client.Get<MoviesResponse>("/cached/movies");

            Assert.That(response.Movies.Count, Is.EqualTo(ResetMoviesService.Top5Movies.Count));
        }

        /// <summary>Can call cached web service with jsonp.</summary>
        [Test]
		public void Can_call_Cached_WebService_with_JSONP()
		{
			var url = Config.NServiceKitBaseUri.CombineWith("/cached/movies?callback=cb");
			var jsonp = url.GetJsonFromUrl();
			Assert.That(jsonp.StartsWith("cb("));
		}
	}
}