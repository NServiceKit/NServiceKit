using Funq;
using NUnit.Framework;
using NServiceKit.ServiceInterface.Admin;
using NServiceKit.Text;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using System.Runtime.Serialization;
using NServiceKit.Service;
using NServiceKit.Messaging;
using System.ServiceModel.Channels;
using System;
using System.Diagnostics;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A buffered request tests.</summary>
    [TestFixture]
    public class BufferedRequestTests
    {
        private BufferedRequestAppHost appHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new BufferedRequestAppHost();
            appHost.Init();
            appHost.Start(Config.AbsoluteBaseUri);
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        /// <summary>Bufferred request allows rereading of request input stream.</summary>
        [Test]
        public void BufferredRequest_allows_rereading_of_Request_InputStream()
        {
            appHost.LastRequestBody = null;
            appHost.UseBufferredStream = true;

            var client = new JsonServiceClient(Config.NServiceKitBaseUri);
            var request = new MyRequest { Data = "RequestData" };
            var response = client.Post(request);

            Assert.That(response.Data, Is.EqualTo(request.Data));
            Assert.That(appHost.LastRequestBody, Is.EqualTo(request.ToJson()));
        }

        /// <summary>Cannot reread request input stream without bufferring.</summary>
        [Test]
        public void Cannot_reread_Request_InputStream_without_bufferring()
        {
            appHost.LastRequestBody = null;
            appHost.UseBufferredStream = false;

            var client = new JsonServiceClient(Config.NServiceKitBaseUri);
            var request = new MyRequest { Data = "RequestData" };

            var response = client.Post(request);

            Assert.That(appHost.LastRequestBody, Is.EqualTo(request.ToJson()));
            Assert.That(response.Data, Is.Null);
        }

        /// <summary>Cannot see request body in request logger without bufferring.</summary>
        [Test]
        public void Cannot_see_RequestBody_in_RequestLogger_without_bufferring()
        {
            appHost.LastRequestBody = null;
            appHost.UseBufferredStream = false;

            var client = new JsonServiceClient(Config.NServiceKitBaseUri);
            var request = new MyRequest { Data = "RequestData" };

            var response = client.Post(request);

            Assert.That(appHost.LastRequestBody, Is.EqualTo(request.ToJson()));
            Assert.That(response.Data, Is.Null);

            var requestLogger = appHost.TryResolve<IRequestLogger>();
            var lastEntry = requestLogger.GetLatestLogs(1);
            Assert.That(lastEntry[0].RequestBody, Is.Null);
        }
    }

    /// <summary>A buffered request logger tests.</summary>
    [TestFixture]
    public class BufferedRequestLoggerTests
    {
        private BufferedRequestAppHost appHost;
        MyRequest request = new MyRequest { Data = "RequestData" };

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new BufferedRequestAppHost { EnableRequestBodyTracking = true };
            appHost.Init();
            appHost.Start(Config.AbsoluteBaseUri);
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        /// <summary>Can see request body in request logger when enable request body tracking.</summary>
        [Test]
        public void Can_see_RequestBody_in_RequestLogger_when_EnableRequestBodyTracking()
        {
            var logBody = Run(new JsonServiceClient(Config.NServiceKitBaseUri));
            Assert.That(appHost.LastRequestBody, Is.EqualTo(request.ToJson()));
            Assert.That(logBody, Is.EqualTo(request.ToJson()));
        }

        /// <summary>Can see request body in request logger when enable request body tracking SOAP 12.</summary>
        [Test]
        public void Can_see_RequestBody_in_RequestLogger_when_EnableRequestBodyTracking_Soap12()
        {
            const string soap12start = @"<s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"" xmlns:a=""http://www.w3.org/2005/08/addressing""><s:Header><a:Action s:mustUnderstand=""1"">MyRequest</a:Action><a:MessageID>urn:uuid:";
            const string soap12end = "<Data>RequestData</Data></MyRequest></s:Body></s:Envelope>";

            var logBody = Run(new Soap12ServiceClient(Config.NServiceKitBaseUri));

            Assert.That(appHost.LastRequestBody, Is.StringStarting(soap12start));
            Assert.That(appHost.LastRequestBody, Is.StringEnding(soap12end));
            Assert.That(logBody, Is.StringStarting(soap12start));
            Assert.That(logBody, Is.StringEnding(soap12end));
        }


        /// <summary>Can see request body in request logger when enable request body tracking SOAP 11.</summary>
        [Test]
        public void Can_see_RequestBody_in_RequestLogger_when_EnableRequestBodyTracking_Soap11()
        {
            const string soap11 = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body><MyRequest xmlns=""http://schemas.NServiceKit.net/types"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><Data>RequestData</Data></MyRequest></s:Body></s:Envelope>";

            var logBody = Run(new Soap11ServiceClient(Config.NServiceKitBaseUri));
            Assert.That(appHost.LastRequestBody, Is.EqualTo(soap11));
            Assert.That(logBody, Is.EqualTo(soap11));
        }

        
        string Run(IServiceClient client)
        {
            var requestLogger = appHost.TryResolve<IRequestLogger>();
            appHost.LastRequestBody = null;
            appHost.UseBufferredStream = false;

            var response = client.Send(request);
            //Debug.WriteLine(appHost.LastRequestBody);

            Assert.That(response.Data, Is.EqualTo(request.Data));

            var lastEntry = requestLogger.GetLatestLogs(int.MaxValue);
            return lastEntry[lastEntry.Count - 1].RequestBody;
        }
        
    }

    /// <summary>A buffered request application host.</summary>
    public class BufferedRequestAppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.BufferedRequestAppHost class.</summary>
        public BufferedRequestAppHost() : base(typeof(BufferedRequestTests).Name, typeof(MyService).Assembly) { }

        /// <summary>Gets or sets the last request body.</summary>
        ///
        /// <value>The last request body.</value>
        public string LastRequestBody { get; set; }

        /// <summary>Gets or sets a value indicating whether this object use bufferred stream.</summary>
        ///
        /// <value>true if use bufferred stream, false if not.</value>
        public bool UseBufferredStream { get; set; }

        /// <summary>Gets or sets a value indicating whether the request body tracking is enabled.</summary>
        ///
        /// <value>true if enable request body tracking, false if not.</value>
        public bool EnableRequestBodyTracking { get; set; }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container)
        {
            PreRequestFilters.Add((httpReq, httpRes) => {
                if (UseBufferredStream)
                    httpReq.UseBufferedStream = UseBufferredStream;

                LastRequestBody = null;
                LastRequestBody = httpReq.GetRawBody();
            });

            Plugins.Add(new RequestLogsFeature { EnableRequestBodyTracking = EnableRequestBodyTracking });
        }
    }

    /// <summary>my request.</summary>
    [DataContract]
    public class MyRequest : IReturn<MyRequest>
    {
        /// <summary>Gets or sets the data.</summary>
        ///
        /// <value>The data.</value>
        [DataMember]
        public string Data { get; set; }
    }

    /// <summary>my service.</summary>
    public class MyService : IService
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(MyRequest request)
        {
            return request;
        }
    }
}
