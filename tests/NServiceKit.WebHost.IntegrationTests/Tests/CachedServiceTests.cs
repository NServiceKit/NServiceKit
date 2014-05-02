using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Plugins.ProtoBuf;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A cached service tests.</summary>
	[TestFixture]
	public class CachedServiceTests
	{
        /// <summary>Executes the before each test action.</summary>
        [TestFixtureSetUp]
        public void OnBeforeEachTest()
        {
            var jsonClient = new JsonServiceClient(Config.NServiceKitBaseUri);
            jsonClient.Post<ResetMoviesResponse>("reset-movies", new ResetMovies());
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

        /// <summary>Can call cached web service with prototype buffer without compression.</summary>
        [Test]
        public void Can_call_Cached_WebService_with_ProtoBuf_without_compression()
        {
            var client = new ProtoBufServiceClient(Config.NServiceKitBaseUri);
            client.DisableAutoCompression = true;
            client.Get<MoviesResponse>("/cached/movies");
            var response2 = client.Get<MoviesResponse>("/cached/movies");

            Assert.That(response2.Movies.Count, Is.EqualTo(ResetMoviesService.Top5Movies.Count));
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