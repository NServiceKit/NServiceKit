using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Web.UI;
using Funq;
using NUnit.Framework;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.WebHost.Endpoints.Support.Metadata.Controls;
using NServiceKit.WebHost.Endpoints.Tests.Support;
using NServiceKit.WebHost.Endpoints.Tests.Support.Operations;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>An operation tests application host.</summary>
    public class OperationTestsAppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.OperationTestsAppHost class.</summary>
        public OperationTestsAppHost() : base(typeof(GetCustomer).Name, typeof(GetCustomer).Assembly) { }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container) { }
    }

    /// <summary>An operation tests.</summary>
	[TestFixture]
	public class OperationTests : MetadataTestBase
	{
        private OperationTestsAppHost _appHost;
	    private OperationControl _operationControl;

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            _appHost = new OperationTestsAppHost();
            _appHost.Init();

            _operationControl = new OperationControl
            {
                HttpRequest = new MockHttpRequest {PathInfo = "", RawUrl = "http://localhost:4444/metadata"},
                MetadataConfig = ServiceEndpointsMetadataConfig.Create(""),
                Format = Format.Json,
                HostName = "localhost",
                RequestMessage = "(string)",
                ResponseMessage = "(HttpWebResponse)",
                Title = "Metadata page",
                OperationName = "operationname",
                MetadataHtml = "<p>Operation</p>"
            };
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            _appHost.Dispose();
        }

        /// <summary>Executes the tear down action.</summary>
        [TearDown]
        public void OnTearDown()
        {
            _appHost.Config.WebHostUrl = null;
        }

        /// <summary>Operation control render creates link back to main page using web host URL when set.</summary>
        [Test]
        public void OperationControl_render_creates_link_back_to_main_page_using_WebHostUrl_when_set()
        {
            _appHost.Config.WebHostUrl = "https://host.example.com/_api";

            var stringWriter = new StringWriter();
            _operationControl.Render(new HtmlTextWriter(stringWriter));

            string html = stringWriter.ToString();
            Assert.IsTrue(html.Contains("<a href=\"https://host.example.com/_api/metadata\">&lt;back to all web services</a>"));
        }

        /// <summary>Operation control render creates link back to main page using relative URI when web host URL not set.</summary>
        [Test]
        public void OperationControl_render_creates_link_back_to_main_page_using_relative_uri_when_WebHostUrl_not_set()
        {
            var stringWriter = new StringWriter();
            _operationControl.Render(new HtmlTextWriter(stringWriter));

            string html = stringWriter.ToString();
            Assert.IsTrue(html.Contains("<a href=\"/metadata\">&lt;back to all web services</a>"));
        }

        /// <summary>When culture is turkish operations containing capital i are still visible.</summary>
        [Test]
        public void When_culture_is_turkish_operations_containing_capital_I_are_still_visible()
        {
            Metadata.Add(GetType(), typeof(HelloImage), null);

            using (new CultureSwitch("tr-TR"))
            {
                Assert.IsTrue(Metadata.IsVisible(_operationControl.HttpRequest, Format.Json, "HelloImage"));
            }
        }
	}

    /// <summary>A hello image.</summary>
    [DataContract]
    public class HelloImage
    {
    }

    /// <summary>A culture switch.</summary>
    public class CultureSwitch : IDisposable
    {
        private readonly CultureInfo _currentCulture;

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.CultureSwitch class.</summary>
        ///
        /// <param name="culture">The culture.</param>
        public CultureSwitch(string culture)
        {
            var currentThread = Thread.CurrentThread;
            _currentCulture = currentThread.CurrentCulture;
            var switchCulture = CultureInfo.GetCultureInfo(culture);
            currentThread.CurrentCulture = switchCulture;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = _currentCulture;
        }
    }
}