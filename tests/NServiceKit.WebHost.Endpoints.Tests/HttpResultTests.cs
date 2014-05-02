using System.IO;
using System.Text;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Tests.Mocks;
using NServiceKit.WebHost.Endpoints.Tests.Support;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A HTTP result tests.</summary>
	[TestFixture]
	public class HttpResultTests : TestBase
	{
        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
		protected override void Configure(Funq.Container container) { }

        /// <summary>Can send response text test with custom header.</summary>
		[Test]
		public void Can_send_ResponseText_test_with_Custom_Header()
		{
			var mockResponse = new HttpResponseMock();

			var customText = "<h1>Custom Text</h1>";

			var httpResult = new HttpResult(customText, ContentType.Html) {
				Headers =
				{
					{"X-Custom","Header"}
				}
			};

			var reponseWasAutoHandled = mockResponse.WriteToResponse(httpResult, ContentType.Html);

			Assert.That(reponseWasAutoHandled, Is.True);

			var writtenString = mockResponse.GetOutputStreamAsString();
			Assert.That(writtenString, Is.EqualTo(customText));
			Assert.That(mockResponse.Headers["X-Custom"], Is.EqualTo("Header"));
		}

        /// <summary>Can send response stream test with custom header.</summary>
		[Test]
		public void Can_send_ResponseStream_test_with_Custom_Header()
		{
			var mockResponse = new HttpResponseMock();

			var customText = "<h1>Custom Stream</h1>";
			var customTextBytes = customText.ToUtf8Bytes();
			var ms = new MemoryStream();
			ms.Write(customTextBytes, 0, customTextBytes.Length);


			var httpResult = new HttpResult(ms, ContentType.Html) {
				Headers =
				{
					{"X-Custom","Header"}
				}
			};

			var reponseWasAutoHandled = mockResponse.WriteToResponse(httpResult, ContentType.Html);

			Assert.That(reponseWasAutoHandled, Is.True);

			var writtenString = mockResponse.GetOutputStreamAsString();
			Assert.That(writtenString, Is.EqualTo(customText));
			Assert.That(mockResponse.Headers["X-Custom"], Is.EqualTo("Header"));
		}

        /// <summary>Can send response text test with status description.</summary>
		[Test]
		public void Can_send_ResponseText_test_with_StatusDescription()
		{
			var mockRequest = new MockHttpRequest { ContentType = ContentType.Json };
			var mockRequestContext = new HttpRequestContext(mockRequest, null, new object());
			var mockResponse = new HttpResponseMock();

			var customStatus = "Custom Status Description";

			var httpResult = new HttpResult(System.Net.HttpStatusCode.Accepted, customStatus) {
				RequestContext = mockRequestContext
			};

			var reponseWasAutoHandled = mockResponse.WriteToResponse(httpResult, ContentType.Html);

			Assert.That(reponseWasAutoHandled, Is.True);

			var statusDesc = mockResponse.StatusDescription;
			Assert.That(mockResponse.StatusCode, Is.EqualTo((int)System.Net.HttpStatusCode.Accepted));
			Assert.That(statusDesc, Is.EqualTo(customStatus));
		}

        /// <summary>Can handle null HTTP result status description.</summary>
		[Test]
		public void Can_handle_null_HttpResult_StatusDescription()
		{
			var mockResponse = new HttpResponseMock();

			var httpResult = new HttpResult();
			httpResult.StatusDescription = null;

			mockResponse.WriteToResponse(httpResult, ContentType.Html);

			Assert.IsNotNull(mockResponse.StatusDescription);
		}
	}

}