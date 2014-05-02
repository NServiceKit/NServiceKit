using System;
using System.Net;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.ServiceClient.Web;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A jsonp tests.</summary>
	public class JsonpTests
	{
        /// <summary>The listening on.</summary>
		protected const string ListeningOn = "http://localhost:82/";

		ExampleAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
			LogManager.LogFactory = new ConsoleLogFactory();

			appHost = new ExampleAppHostHttpListener();
			appHost.Init();
			appHost.Start(ListeningOn);
		}

        /// <summary>Executes the test fixture tear down action.</summary>
		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			Dispose();
		}

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			if (appHost == null) return;
			appHost.Dispose();
			appHost = null;
		}

        /// <summary>Can get single movie using rest client with jsonp.</summary>
		[Test]
		public void Can_GET_single_Movie_using_RestClient_with_JSONP()
		{
			var url = ListeningOn + "movies/1?callback=cb";
			string response;

			var webReq = (HttpWebRequest)WebRequest.Create(url);
			webReq.Accept = "*/*";
			using (var webRes = webReq.GetResponse())
			{
				Assert.That(webRes.ContentType, Is.StringStarting(ContentType.JavaScript));
				response = webRes.DownloadText();
			}

			Assert.That(response, Is.Not.Null, "No response received");
			Console.WriteLine(response);
			Assert.That(response, Is.StringStarting("cb("));
			Assert.That(response, Is.StringEnding(")"));
			Assert.That(response.Length, Is.GreaterThan(50));
		} 
	}
}