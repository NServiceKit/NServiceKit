using System.Net;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.Text;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A request filter tests.</summary>
	[TestFixture]
	public class RequestFilterTests
	{
		private const string ServiceClientBaseUri = Config.NServiceKitBaseUri;

        /// <summary>Does return bare 401 status code.</summary>
        ///
        /// <exception cref="401">Thrown when a 401 error condition occurs.</exception>
		[Test]
		public void Does_return_bare_401_StatusCode()
		{
			try
			{
				var webRequest = (HttpWebRequest)WebRequest.Create(ServiceClientBaseUri 
					+ "/json/syncreply/RequestFilter?StatusCode=401");

				var webResponse = (HttpWebResponse)webRequest.GetResponse();
				webResponse.Method.Print();
				Assert.Fail("Should throw 401 WebException");
			}
			catch (WebException ex)
			{
                var httpResponse = (HttpWebResponse)ex.Response;
                Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
			}
		}

        /// <summary>Does return bare 401 with authentication required header.</summary>
        ///
        /// <exception cref="401">Thrown when a 401 error condition occurs.</exception>
		[Test]
		public void Does_return_bare_401_with_AuthRequired_header()
		{
			try
			{
				var webRequest = (HttpWebRequest)WebRequest.Create(ServiceClientBaseUri
					+ "/json/syncreply/RequestFilter?StatusCode=401"
					+ "&HeaderName=" + HttpHeaders.WwwAuthenticate
					+ "&HeaderValue=" + "Basic realm=\"Auth Required\"".UrlEncode());

				var webResponse = (HttpWebResponse)webRequest.GetResponse();
				webResponse.Method.Print();
				Assert.Fail("Should throw 401 WebException");
			}
			catch (WebException ex)
			{
                var httpResponse = (HttpWebResponse)ex.Response;
                Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

				Assert.That(ex.Response.Headers[HttpHeaders.WwwAuthenticate],
					Is.EqualTo("Basic realm=\"Auth Required\""));
			}
		}

        /// <summary>Does return send 401 for access to i secure requests.</summary>
        ///
        /// <exception cref="401">Thrown when a 401 error condition occurs.</exception>
		[Test]
		public void Does_return_send_401_for_access_to_ISecure_requests()
		{
			try
			{
				var webRequest = (HttpWebRequest)WebRequest.Create(ServiceClientBaseUri
					+ "/json/syncreply/Secure?SessionId=175BEA29-DC79-4555-BD42-C4DD5D57A004");

				var webResponse = (HttpWebResponse)webRequest.GetResponse();
				webResponse.Method.Print();
				Assert.Fail("Should throw 401 WebException");
			}
			catch (WebException ex)
			{
                var httpResponse = (HttpWebResponse)ex.Response;
                Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
			}
		}
	}
}