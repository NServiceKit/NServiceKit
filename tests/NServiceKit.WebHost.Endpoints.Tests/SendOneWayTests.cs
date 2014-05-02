using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using NServiceKit.Common.Web;
using NServiceKit.Service;
using NServiceKit.ServiceHost;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceInterface;
using NServiceKit.CacheAccess;
using NServiceKit.CacheAccess.Providers;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Utils;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A delete one way request.</summary>
    [Route("/onewayrequest", "DELETE")]
    public class DeleteOneWayRequest : IReturnVoid
    {
        /// <summary>Gets or sets the prefix.</summary>
        ///
        /// <value>The prefix.</value>
        public string Prefix { get; set; }
    }

    /// <summary>A post one way request.</summary>
    [Route("/onewayrequest", "POST")]
    [Route("/onewayrequest", "PUT")]
    [DataContract]
    public class PostOneWayRequest : IReturnVoid
    {
        /// <summary>Gets or sets the prefix.</summary>
        ///
        /// <value>The prefix.</value>
        [DataMember]
        public string Prefix { get; set; }

        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
        [DataMember(Name = "some-title")]
        public string Title { get; set; }
    }    

    /// <summary>An one way service.</summary>
    public class OneWayService : ServiceInterface.Service
    {
        /// <summary>Gets or sets the last result.</summary>
        ///
        /// <value>The last result.</value>
        public static string LastResult { get; set; }

        /// <summary>Deletes the given oneWayRequest.</summary>
        ///
        /// <param name="oneWayRequest">The one way request to put.</param>
        public void Delete(DeleteOneWayRequest oneWayRequest)
        {
            LastResult = oneWayRequest.Prefix + " " + Request.HttpMethod;
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="oneWayRequest">The one way request to put.</param>
        public void Post(PostOneWayRequest oneWayRequest)
        {
            LastResult = oneWayRequest.Prefix + " " + Request.HttpMethod + oneWayRequest.Title;
        }

        /// <summary>Puts the given one way request.</summary>
        ///
        /// <param name="oneWayRequest">The one way request to put.</param>
        public void Put(PostOneWayRequest oneWayRequest)
        {
            Post(oneWayRequest);
        }
    }

    /// <summary>An one way service tests.</summary>
    [TestFixture]
    public class OneWayServiceTests
    {
        private const string ListeningOn = "http://localhost:8023/";
        private const string ServiceClientBaseUri = "http://localhost:8023/";

        /// <summary>An one way service application host HTTP listener.</summary>
        public class OneWayServiceAppHostHttpListener
            : AppHostHttpListenerBase
        {

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.OneWayServiceTests.OneWayServiceAppHostHttpListener class.</summary>
            public OneWayServiceAppHostHttpListener()
                : base("", typeof(OneWayService).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Funq.Container container)
            {
            }
        }

        OneWayServiceAppHostHttpListener appHost;
        private IRestClient client;

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            appHost = new OneWayServiceAppHostHttpListener();
            appHost.Init();
            appHost.Start(ListeningOn);

            client = new JsonServiceClient(ServiceClientBaseUri);
            OneWayService.LastResult = null;
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            appHost.Dispose();
        }


        /// <summary>Deletes this object.</summary>
        [Test]
        public void Delete()
        {
            client.Delete(new DeleteOneWayRequest() { Prefix = "Delete" });
            Assert.That(OneWayService.LastResult, Is.EqualTo("Delete DELETE"));

        }

        /// <summary>Send this message.</summary>
        [Test]
        public void Send()
        {
            client.Post(new PostOneWayRequest() { Prefix = "Post" });
            Assert.That(OneWayService.LastResult, Is.EqualTo("Post POST"));
        }

        /// <summary>Should respect data member name.</summary>
        [Test]
        public void Should_Respect_DataMember_Name()
        {
            GetResponse(ServiceClientBaseUri + "onewayrequest", "{\"some-title\": \"right\", \"Title\": \"wrong\"}");
            Assert.That(OneWayService.LastResult, Is.EqualTo(" PUTright"));
        }

        /// <summary>Gets a response.</summary>
        ///
        /// <param name="url"> URL of the document.</param>
        /// <param name="json">The JSON.</param>
        ///
        /// <returns>The response.</returns>
        public static string GetResponse(String url, string json)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "PUT";
            var formDataBytes = Encoding.UTF8.GetBytes(json);
            webRequest.ContentLength = formDataBytes.Length;
            webRequest.ContentType = "application/json";
            webRequest.GetRequestStream().Write(formDataBytes, 0, formDataBytes.Length);
            var webResponse = webRequest.GetResponse();
            return new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
        }

    }
}
