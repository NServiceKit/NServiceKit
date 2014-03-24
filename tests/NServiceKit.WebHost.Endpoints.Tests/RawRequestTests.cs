using System;
using System.IO;
using System.Net;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
	[RestService("/rawrequest")]
	public class RawRequest : IRequiresRequestStream
	{
		public Stream RequestStream { get; set; }
	}

	public class RawRequestResponse
	{
		public string Result { get; set; }
	}

	public class RawRequestService : IService<RawRequest>
	{
		public object Execute(RawRequest request)
		{
			var rawRequest = request.RequestStream.ToUtf8String();
			return new RawRequestResponse { Result = rawRequest };
		}
	}

	[TestFixture]
	public class RawRequestTests 
	{
		static class Config
		{
			public const string AbsoluteBaseUri = "http://localhost:82/";
		}

		ExampleAppHostHttpListener appHost;
		
		[TestFixtureSetUp]
		public void OnTestFixtureStartUp() 
		{
			appHost = new ExampleAppHostHttpListener();
			appHost.Init();
			appHost.Start(Config.AbsoluteBaseUri);

			System.Console.WriteLine("RawRequestTests Created at {0}, listening on {1}",
									 DateTime.Now, Config.AbsoluteBaseUri);
		}

		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			appHost.Dispose();
			appHost = null;
		}

		[Test]
		public void Can_POST_raw_request()
		{
			var rawData = "<<(( 'RAW_DATA' ))>>";
			var requestUrl = Config.AbsoluteBaseUri + "/rawrequest";
			var json = requestUrl.PutToUrl(rawData, ContentType.Json);
			var response = json.FromJson<RawRequestResponse>();
			Assert.That(response.Result, Is.EqualTo(rawData));
		}

		[Test]
		public void Can_PUT_raw_request()
		{
			var rawData = "<<(( 'RAW_DATA' ))>>";
			var requestUrl = Config.AbsoluteBaseUri + "/rawrequest";
			var json = requestUrl.PutToUrl(rawData, ContentType.Json);
			var response = json.FromJson<RawRequestResponse>();
			Assert.That(response.Result, Is.EqualTo(rawData));
		}

	}

}