using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;
using NUnit.Framework;
using System.Net;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>
    /// Note: These tests don't test NServiceKit when its mounted at /api
    /// </summary>
    [TestFixture]
    public class AppHostBaseTests
    {
        private const string BasePath = Config.AbsoluteBaseUri;

        [Test]
        public void Root_path_redirects_to_metadata_page()
        {
            var html = Config.NServiceKitBaseUri.GetStringFromUrl();
            Assert.That(html.Contains("The following operations are supported."));
        }

        [Test]
        public void Can_download_webpage_html_page()
        {
            var html = (BasePath + "webpage.html").GetStringFromUrl();
            Assert.That(html.Contains("Default index NServiceKit.WebHost.Endpoints.Tests page"));
        }

        [Test]
        public void Gets_404_on_non_existant_page()
        {
            var webRes = (BasePath + "nonexistant.html").GetErrorResponse();
            Assert.That(webRes.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void Gets_404_3_on_page_with_non_whitelisted_extension()
        {
            var webRes = (BasePath + "api/webpage.forbidden").GetErrorResponse();
            Assert.That(webRes.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

    }
}