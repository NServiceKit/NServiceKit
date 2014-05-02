using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using NUnit.Framework;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A request and path resolution tests.</summary>
	[TestFixture]
	public class RequestAndPathResolutionTests
		: TestBase
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Tests.RequestAndPathResolutionTests class.</summary>
		public RequestAndPathResolutionTests()
			: base(Config.NServiceKitBaseUri, typeof(ReverseService).Assembly)
		{
		}

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
		protected override void Configure(Funq.Container container) { }

        /// <summary>Executes the before test action.</summary>
		[SetUp]
		public void OnBeforeTest()
		{
			base.OnBeforeEachTest();
		    RegisterConfig();
		}

        private void RegisterConfig()
        {
            EndpointHost.CatchAllHandlers.Add(new PredefinedRoutesFeature().ProcessRequest);
            EndpointHost.CatchAllHandlers.Add(new MetadataFeature().ProcessRequest);
        }

        /// <summary>Can process default request.</summary>
		[Test]
		public void Can_process_default_request()
		{
			var request = (EchoRequest)ExecutePath("/Xml/SyncReply/EchoRequest");
			Assert.That(request, Is.Not.Null);
		}

        /// <summary>Can process default case insensitive request.</summary>
		[Test]
		public void Can_process_default_case_insensitive_request()
		{
			var request = (EchoRequest)ExecutePath("/xml/syncreply/echorequest");
			Assert.That(request, Is.Not.Null);
		}

        /// <summary>Can process default request with query string.</summary>
		[Test]
		public void Can_process_default_request_with_queryString()
		{
			var request = (EchoRequest)ExecutePath("/Xml/SyncReply/EchoRequest?Id=1&String=Value");
			Assert.That(request, Is.Not.Null);
			Assert.That(request.Id, Is.EqualTo(1));
			Assert.That(request.String, Is.EqualTo("Value"));
		}
	}
}