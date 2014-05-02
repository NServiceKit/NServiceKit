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
    /// <summary>A raw request.</summary>
	[Route("/rawrequest")]
	public class RawRequest : IRequiresRequestStream
	{
        /// <summary>The raw Http Request Input Stream.</summary>
        ///
        /// <value>The request stream.</value>
		public Stream RequestStream { get; set; }
	}

    /// <summary>A raw request response.</summary>
	public class RawRequestResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		public string Result { get; set; }
	}

    /// <summary>A raw request service.</summary>
	public class RawRequestService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(RawRequest request)
		{
			var rawRequest = request.RequestStream.ToUtf8String();
			return new RawRequestResponse { Result = rawRequest };
		}
	}

    /// <summary>A raw request tests.</summary>
	[TestFixture]
	public class RawRequestTests 
	{
        /// <summary>Can post raw request.</summary>
		[Test]
		public void Can_POST_raw_request()
		{
			var rawData = "<<(( 'RAW_DATA' ))>>";
			var requestUrl = Config.NServiceKitBaseUri + "/rawrequest";
            var json = requestUrl.PutStringToUrl(rawData, contentType: ContentType.PlainText, acceptContentType: ContentType.Json);
			var response = json.FromJson<RawRequestResponse>();
			Assert.That(response.Result, Is.EqualTo(rawData));
		}

        /// <summary>Can put raw request.</summary>
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