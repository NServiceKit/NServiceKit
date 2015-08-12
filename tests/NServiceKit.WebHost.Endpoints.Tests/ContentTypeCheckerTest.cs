using NUnit.Framework;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;
using System.Collections.Generic;
using NServiceKit;
using System.Net;
using System.IO;
using System;
using NServiceKit.ServiceClient.Web;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary> Tests the content type checker feature </summary>
    [TestFixture]
    public class ContentTypeCheckerTest
    {
        private const string HostingAt = "http://localhost:12345/";
        private List<string> AcceptedRequests = new List<string> {"application/Json"};
        ExampleAppHostHttpListener appHost;
        
        [TestFixtureSetUp]
        public void OnTestFixtureSetup() {
            appHost = new ExampleAppHostHttpListener();
            appHost.Plugins.Add(new ContentTypeCheckerFeature(AcceptedRequests));
            appHost.Init();
            appHost.Start(HostingAt);
        }

        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {

            appHost.Dispose();
        }
  
        [Test]
        public void CorrectContentTypeCheck() 
        {
            Assert.DoesNotThrow(new TestDelegate(CorrectRequest));
        }

        [Test]
        public void IncorrectContentTypeCheck()
        {
            Assert.Throws(typeof(WebServiceException), new TestDelegate(IncorrectRequest));
        }
        /// <summary>Sends a request of contentype application/json</summary>
        private void CorrectRequest()
        {
            var client = new JsonServiceClient(HostingAt);
            var request = new MyRequest {Data = "RequestData"};
            var response = client.Post(request);
        }
        /// <summary>Sends a request of contentype application/xml </summary>
        private void IncorrectRequest()
        {
            var client = new XmlServiceClient(HostingAt);
            var request = new MyRequest {Data = "RequestData"};
            var response = client.Post(request);
        }
    }
}
