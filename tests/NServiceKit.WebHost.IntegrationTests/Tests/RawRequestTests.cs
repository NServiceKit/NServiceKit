using System;
using System.IO;
using System.Net;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
	[Route("/rawrequest")]
	public class RawRequest : IRequiresRequestStream
	{
		public Stream RequestStream { get; set; }
	}

	public class RawRequestResponse
	{
		public string Result { get; set; }
	}

	public class RawRequestService : ServiceInterface.Service
	{
		public object Any(RawRequest request)
		{
			var rawRequest = request.RequestStream.ToUtf8String();
			return new RawRequestResponse { Result = rawRequest };
		}
	}

	[TestFixture]
	public class RawRequestTests 
	{
		[Test]
		public void Can_POST_raw_request()
		{
			var rawData = "<<(( 'RAW_DATA' ))>>";
			var requestUrl = Config.NServiceKitBaseUri + "/rawrequest";
            var json = requestUrl.PutStringToUrl(rawData, contentType: ContentType.PlainText, acceptContentType: ContentType.Json);
			var response = json.FromJson<RawRequestResponse>();
			Assert.That(response.Result, Is.EqualTo(rawData));
		}

		[Test]
		public void Can_PUT_raw_request()
		{
			var rawData = "<<(( 'RAW_DATA' ))>>";
			var requestUrl = Config.NServiceKitBaseUri + "/rawrequest";
            var json = requestUrl.PutStringToUrl(rawData, contentType: ContentType.PlainText, acceptContentType: ContentType.Json);
			var response = json.FromJson<RawRequestResponse>();
			Assert.That(response.Result, Is.EqualTo(rawData));
		}

	}

}