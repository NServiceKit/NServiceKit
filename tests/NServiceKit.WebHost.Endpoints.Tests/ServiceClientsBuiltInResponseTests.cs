using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Funq;
using NUnit.Framework;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A poco.</summary>
    [Route("/poco/{Text}")]
    public class Poco : IReturn<PocoResponse>
    {
        /// <summary>Gets or sets the text.</summary>
        ///
        /// <value>The text.</value>
        public string Text { get; set; }
    }

    /// <summary>A poco response.</summary>
    public class PocoResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public string Result { get; set; }
    }

    
    /// <summary>A headers.</summary>
    [Route("/headers/{Text}")]
    public class Headers : IReturn<HttpWebResponse>
    {
        /// <summary>Gets or sets the text.</summary>
        ///
        /// <value>The text.</value>
        public string Text { get; set; }
    }

    /// <summary>A strings.</summary>
    [Route("/strings/{Text}")]
    public class Strings : IReturn<string>
    {
        /// <summary>Gets or sets the text.</summary>
        ///
        /// <value>The text.</value>
        public string Text { get; set; }
    }

    /// <summary>A bytes.</summary>
    [Route("/bytes/{Text}")]
    public class Bytes : IReturn<byte[]>
    {
        /// <summary>Gets or sets the text.</summary>
        ///
        /// <value>The text.</value>
        public string Text { get; set; }
    }

    /// <summary>A streams.</summary>
    [Route("/streams/{Text}")]
    public class Streams : IReturn<Stream>
    {
        /// <summary>Gets or sets the text.</summary>
        ///
        /// <value>The text.</value>
        public string Text { get; set; }
    }

    /// <summary>A stream writers.</summary>
    [Route("/streamwriter/{Text}")]
    public class StreamWriters : IReturn<Stream>
    {
        /// <summary>Gets or sets the text.</summary>
        ///
        /// <value>The text.</value>
        public string Text { get; set; }
    }

    /// <summary>A built in types service.</summary>
    public class BuiltInTypesService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An IStreamWriter.</returns>
        public PocoResponse Any(Poco request)
        {
            return new PocoResponse { Result = "Hello, " + (request.Text ?? "World!") };
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        public void Any(Headers request)
        {
            base.Response.AddHeader("X-Response", request.Text);
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An IStreamWriter.</returns>
        public string Any(Strings request)
        {
            return "Hello, " + (request.Text ?? "World!");
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An IStreamWriter.</returns>
        public byte[] Any(Bytes request)
        {
            return new Guid(request.Text).ToByteArray();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An IStreamWriter.</returns>
        public byte[] Any(Streams request)
        {
            return new Guid(request.Text).ToByteArray();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An IStreamWriter.</returns>
        public IStreamWriter Any(StreamWriters request)
        {
            return new StreamWriterResult(new Guid(request.Text).ToByteArray());
        }
    }

    /// <summary>Encapsulates the result of a stream writer.</summary>
    public class StreamWriterResult : IStreamWriter
    {
        private byte[] result;

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.StreamWriterResult class.</summary>
        ///
        /// <param name="result">The result.</param>
        public StreamWriterResult(byte[] result)
        {
            this.result = result;
        }

        /// <summary>Writes to.</summary>
        ///
        /// <param name="responseStream">The response stream.</param>
        public void WriteTo(Stream responseStream)
        {
            responseStream.Write(result, 0, result.Length);
        }
    }
    
    /// <summary>A built in types application host.</summary>
    public class BuiltInTypesAppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.BuiltInTypesAppHost class.</summary>
        public BuiltInTypesAppHost() : base(typeof(BuiltInTypesAppHost).Name, typeof(BuiltInTypesService).Assembly) { }

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
        public override void Configure(Container container) {}
    }

    /// <summary>A service clients built in response tests.</summary>
    [TestFixture]
    public class ServiceClientsBuiltInResponseTests
    {
        private BufferedRequestAppHost appHost;

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

        /// <summary>The rest clients.</summary>
        protected static IRestClient[] RestClients = 
		{
			new JsonServiceClient(Config.AbsoluteBaseUri),
			new XmlServiceClient(Config.AbsoluteBaseUri),
			new JsvServiceClient(Config.AbsoluteBaseUri),
		};

        /// <summary>The service clients.</summary>
        protected static IServiceClient[] ServiceClients = 
            RestClients.OfType<IServiceClient>().ToArray();

        /// <summary>Can download poco response.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Poco_response(IRestClient client)
        {
            PocoResponse response = client.Get(new Poco { Text = "Test" });

            Assert.That(response.Result, Is.EqualTo("Hello, Test"));
        }

        /// <summary>Can download poco response as string.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Poco_response_as_string(IRestClient client)
        {
            string response = client.Get<string>("/poco/Test");

            Assert.That(response, Is.StringContaining("Hello, Test"));
        }

        /// <summary>Can download poco response as bytes.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Poco_response_as_bytes(IRestClient client)
        {
            byte[] response = client.Get<byte[]>("/poco/Test");

            Assert.That(response.FromUtf8Bytes(), Is.StringContaining("Hello, Test"));
        }

        /// <summary>Can download poco response as stream.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Poco_response_as_Stream(IRestClient client)
        {
            Stream response = client.Get<Stream>("/poco/Test");
            using (response)
            {
                var bytes = response.ReadFully();
                Assert.That(bytes.FromUtf8Bytes(), Is.StringContaining("Hello, Test"));
            }
        }

        /// <summary>Can download poco response as poco response.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Poco_response_as_PocoResponse(IRestClient client)
        {
            HttpWebResponse response = client.Get<HttpWebResponse>("/poco/Test");

            using (var stream = response.GetResponseStream())
            using (var sr = new StreamReader(stream))
            {
                Assert.That(sr.ReadToEnd(), Is.StringContaining("Hello, Test"));
            }
        }

        /// <summary>Can download headers response.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Headers_response(IRestClient client)
        {
            HttpWebResponse response = client.Get(new Headers { Text = "Test" });
            Assert.That(response.Headers["X-Response"], Is.EqualTo("Test"));
        }

        /// <summary>Can download headers response asynchronous.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Headers_response_Async(IServiceClient client)
        {
            //Note: HttpWebResponse is returned before any response is read, so it's ideal point for streaming in app code

            HttpWebResponse response = null;
            client.GetAsync(new Headers { Text = "Test" }, r => response = r,
                (r, ex) => Assert.Fail(ex.Message));

            Thread.Sleep(2000);

            Assert.That(response.Headers["X-Response"], Is.EqualTo("Test"));
        }

        /// <summary>Can download strings response.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Strings_response(IRestClient client)
        {
            string response = client.Get(new Strings { Text = "Test" });
            Assert.That(response, Is.EqualTo("Hello, Test"));
        }

        /// <summary>Can download strings response asynchronous.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Strings_response_Async(IServiceClient client)
        {
            string response = null;
            client.GetAsync(new Strings { Text = "Test" }, r => response = r,
                (r, ex) => Assert.Fail(ex.Message));

            Thread.Sleep(2000);

            Assert.That(response, Is.EqualTo("Hello, Test"));
        }

        /// <summary>Can download bytes response.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Bytes_response(IRestClient client)
        {
            var guid = Guid.NewGuid();
            byte[] response = client.Get(new Bytes { Text = guid.ToString() });
            Assert.That(new Guid(response), Is.EqualTo(guid));
        }

        /// <summary>Can download bytes response asynchronous.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Bytes_response_Async(IServiceClient client)
        {
            byte[] bytes = null;
            var guid = Guid.NewGuid();
            client.GetAsync(new Bytes { Text = guid.ToString() }, r => bytes = r,
                (r, ex) => Assert.Fail(ex.Message));

            Thread.Sleep(2000);

            Assert.That(new Guid(bytes), Is.EqualTo(guid));
        }

        /// <summary>Can download streams response.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Streams_response(IRestClient client)
        {
            var guid = Guid.NewGuid();
            Stream response = client.Get(new Streams { Text = guid.ToString() });
            using (response)
            {
                var bytes = response.ReadFully();
                Assert.That(new Guid(bytes), Is.EqualTo(guid));
            }
        }

        /// <summary>Can download streams response asynchronous.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_Streams_response_Async(IServiceClient client)
        {
            //Note: The populated MemoryStream which bufferred the response is returned (i.e. after the response is read async-ly)

            byte[] bytes = null;
            var guid = Guid.NewGuid();
            client.GetAsync(new Streams { Text = guid.ToString() }, stream => {
                using (stream)
                {
                    bytes = stream.ReadFully();
                }
            }, (stream, ex) => Assert.Fail(ex.Message));

            Thread.Sleep(2000);

            Assert.That(new Guid(bytes), Is.EqualTo(guid));
        }

        /// <summary>Can download stream wroter response.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Can_download_StreamWroter_response(IRestClient client)
        {
            var guid = Guid.NewGuid();
            Stream response = client.Get(new StreamWriters { Text = guid.ToString() });
            using (response)
            {
                var bytes = response.ReadFully();
                Assert.That(new Guid(bytes), Is.EqualTo(guid));
            }
        }
         
    }
}