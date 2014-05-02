using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost.Tests.Formats;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Formats;

namespace NServiceKit.ServiceHost.Tests
{
    /// <summary>A request context extensions test.</summary>
    [TestFixture]
    public class RequestContextExtensionsTest
    {
        /// <summary>Can optimize HTML result with to optimized result.</summary>
        [Test]
        public void Can_optimize_html_result_with_ToOptimizedResult()
        {
            CanOptimizeResult("text/html", new HtmlFormat());
        }

        /// <summary>Can optimize CSV result with to optimized result.</summary>
        [Test]
        public void Can_optimize_csv_result_with_ToOptimizedResult()
        {
            CanOptimizeResult("text/csv", new CsvFormat());
        }

        /// <summary>Can optimize JSON result with to optimized result.</summary>
        [Test]
        public void Can_optimize_json_result_with_ToOptimizedResult()
        {
            CanOptimizeResult(ContentType.Json, null);
        }

        /// <summary>Can optimize XML result with to optimized result.</summary>
        [Test]
        public void Can_optimize_xml_result_with_ToOptimizedResult()
        {
            CanOptimizeResult(ContentType.Xml, null);
        }

        /// <summary>Can optimize jsv result with to optimized result.</summary>
        [Test]
        public void Can_optimize_jsv_result_with_ToOptimizedResult()
        {
            CanOptimizeResult(ContentType.Jsv, null);
        }

        private static void CanOptimizeResult(string contentType, IPlugin pluginFormat)
        {
            var dto = new TestDto {Name = "test"};

            var httpReq = new MockHttpRequest();
            httpReq.Headers.Add(HttpHeaders.AcceptEncoding, "gzip,deflate,sdch");
            httpReq.ResponseContentType = contentType;
            var httpRes = new ViewTests.MockHttpResponse();

            var httpRequestContext = new HttpRequestContext(httpReq, httpRes, dto);

            var appHost = new TestAppHost();
            if (pluginFormat != null) pluginFormat.Register(appHost);

            EndpointHost.ContentTypeFilter = appHost.ContentTypeFilters;

            object result = httpRequestContext.ToOptimizedResult(dto);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is CompressedResult);
        }

        /// <summary>A test dto.</summary>
        public class TestDto
        {
            /// <summary>Gets or sets the name.</summary>
            ///
            /// <value>The name.</value>
            public string Name { get; set; }
        }
    }
}