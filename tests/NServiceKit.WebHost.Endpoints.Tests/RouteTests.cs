using Funq;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Formats;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A route tests.</summary>
    [TestFixture]
    public class RouteTests
    {
        private RouteAppHost appHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new RouteAppHost();
            appHost.Init();
            appHost.Start(Config.AbsoluteBaseUri);
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        /// <summary>Can download original route.</summary>
        [Test]
        public void Can_download_original_route()
        {
            var response = Config.AbsoluteBaseUri.CombineWith("/custom/foo")
                .GetStringFromUrl(responseFilter: httpRes =>
                {
                    httpRes.ContentType.Print();
                    Assert.That(httpRes.ContentType.MatchesContentType(ContentType.Html));
                });

            Assert.That(response, Is.StringStarting("<!doctype html>"));
        }

        /// <summary>Can download original route with JSON extension.</summary>
        [Test]
        public void Can_download_original_route_with_json_extension()
        {
            var response = Config.AbsoluteBaseUri.CombineWith("/custom/foo.json")
                .GetStringFromUrl(responseFilter: httpRes =>
                {
                    httpRes.ContentType.Print();
                    Assert.That(httpRes.ContentType.MatchesContentType(ContentType.Json));
                });

            Assert.That(response.ToLower(), Is.EqualTo( "{\"data\":\"foo\"}"));
        }

        /// <summary>Can download original route with XML extension.</summary>
        [Test]
        public void Can_download_original_route_with_xml_extension()
        {
            var response = Config.AbsoluteBaseUri.CombineWith("/custom/foo.xml")
                .GetStringFromUrl(responseFilter: httpRes =>
                {
                    httpRes.ContentType.Print();
                    Assert.That(httpRes.ContentType.MatchesContentType(ContentType.Xml));
                });

            Assert.That(response, Is.EqualTo("<CustomRoute xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.NServiceKit.net/types\"><Data>foo</Data></CustomRoute>"));
        }

        /// <summary>Can download original route with HTML extension.</summary>
        [Test]
        public void Can_download_original_route_with_html_extension()
        {
            var response = Config.AbsoluteBaseUri.CombineWith("/custom/foo.html")
                .GetStringFromUrl(responseFilter: httpRes =>
                {
                    httpRes.ContentType.Print();
                    Assert.That(httpRes.ContentType.MatchesContentType(ContentType.Html));
                });

            Assert.That(response, Is.StringStarting("<!doctype html>"));
        }

        /// <summary>Can download original route with CSV extension.</summary>
        [Test]
        public void Can_download_original_route_with_csv_extension()
        {
            var response = Config.AbsoluteBaseUri.CombineWith("/custom/foo.csv")
                .GetStringFromUrl(responseFilter: httpRes =>
                {
                    httpRes.ContentType.Print();
                    Assert.That(httpRes.ContentType.MatchesContentType(ContentType.Csv));
                });

            Assert.That(response, Is.EqualTo("Data\r\nfoo\r\n"));
        }
    }

    /// <summary>A route application host.</summary>
    public class RouteAppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.RouteAppHost class.</summary>
        public RouteAppHost() : base(typeof(BufferedRequestTests).Name, typeof(CustomRouteService).Assembly) { }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container)
        {
            SetConfig(new EndpointHostConfig {
                AllowRouteContentTypeExtensions = true
            });

            Plugins.Add(new CsvFormat()); //required to allow .csv
        }
    }

    /// <summary>A custom route.</summary>
    [Route("/custom")]
    [Route("/custom/{Data}")]
    public class CustomRoute : IReturn<CustomRoute>
    {
        /// <summary>Gets or sets the data.</summary>
        ///
        /// <value>The data.</value>
        public string Data { get; set; }
    }

    /// <summary>A custom route service.</summary>
    public class CustomRouteService : IService
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(CustomRoute request)
        {
            return request;
        }
    }
}
