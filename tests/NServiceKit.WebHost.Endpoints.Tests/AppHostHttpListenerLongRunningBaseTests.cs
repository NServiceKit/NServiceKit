using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    [TestFixture]
    [Ignore]
    class AppHostHttpListenerLongRunningBaseTests
    {
        private const string ListeningOn = "http://localhost:82/";
        ExampleAppHostHttpListenerLongRunning appHost;

        static AppHostHttpListenerLongRunningBaseTests()
        {
            LogManager.LogFactory = new ConsoleLogFactory();
        }

        /// <summary>Executes the test fixture start up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureStartUp()
        {
            appHost = new ExampleAppHostHttpListenerLongRunning();
            appHost.Init();
            appHost.Start(ListeningOn);

            System.Console.WriteLine("ExampleAppHost Created at {0}, listening on {1}",
                                     DateTime.Now, ListeningOn);
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        /// <summary>Root path redirects to metadata page.</summary>
        [Test]
        public void Root_path_redirects_to_metadata_page()
        {
            var html = ListeningOn.GetStringFromUrl();
            Assert.That(html.Contains("The following operations are supported."));
        }

        /// <summary>Can download webpage HTML page.</summary>
        [Test]
        public void Can_download_webpage_html_page()
        {
            var html = (ListeningOn + "webpage.html").GetStringFromUrl();
            Assert.That(html.Contains("Default index NServiceKit.WebHost.Endpoints.Tests page"));
        }

        /// <summary>Can download requestinfo JSON.</summary>
        [Test]
        public void Can_download_requestinfo_json()
        {
            var html = (ListeningOn + "_requestinfo").GetStringFromUrl();
            Assert.That(html.Contains("\"Host\":"));
        }

        /// <summary>Gets 404 on non existant page.</summary>
        [Test]
        public void Gets_404_on_non_existant_page()
        {
            var webRes = (ListeningOn + "nonexistant.html").GetErrorResponse();
            Assert.That(webRes.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        /// <summary>Gets 403 on page with non whitelisted extension.</summary>
        [Test]
        public void Gets_403_on_page_with_non_whitelisted_extension()
        {
            var webRes = (ListeningOn + "webpage.forbidden").GetErrorResponse();
            Assert.That(webRes.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        /// <summary>Can call get factorial web service.</summary>
        [Test]
        public void Can_call_GetFactorial_WebService()
        {
            var client = new XmlServiceClient(ListeningOn);
            var request = new GetFactorial { ForNumber = 3 };
            var response = client.Send<GetFactorialResponse>(request);

            Assert.That(response.Result, Is.EqualTo(GetFactorialService.GetFactorial(request.ForNumber)));
        }

        /// <summary>Can call jsv debug on get factorial web service.</summary>
        [Test]
        public void Can_call_jsv_debug_on_GetFactorial_WebService()
        {
            const string url = ListeningOn + "jsv/syncreply/GetFactorial?ForNumber=3&debug=true";
            var contents = url.GetStringFromUrl();


            Console.WriteLine("JSV DEBUG: " + contents);

            Assert.That(contents, Is.Not.Null);
        }

        /// <summary>Calling missing web service does not break HTTP listener.</summary>
        [Test]
        public void Calling_missing_web_service_does_not_break_HttpListener()
        {
            var missingUrl = ListeningOn + "missing.html";
            int errorCount = 0;
            try
            {
                missingUrl.GetStringFromUrl();
            }
            catch (Exception ex)
            {
                errorCount++;
                Console.WriteLine("Error [{0}]: {1}", ex.GetType().Name, ex.Message);
            }
            try
            {
                missingUrl.GetStringFromUrl();
            }
            catch (Exception ex)
            {
                errorCount++;
                Console.WriteLine("Error [{0}]: {1}", ex.GetType().Name, ex.Message);
            }

            Assert.That(errorCount, Is.EqualTo(2));
        }

        /// <summary>Can call movies zip web service.</summary>
        [Test]
        public void Can_call_MoviesZip_WebService()
        {
            var client = new JsonServiceClient(ListeningOn);
            var request = new MoviesZip();
            var response = client.Send<MoviesZipResponse>(request);

            Assert.That(response.Movies.Count, Is.GreaterThan(0));
        }

        /// <summary>Calling not implemented method returns 405.</summary>
        ///
        /// <exception cref="405">Thrown when a 405 error condition occurs.</exception>
        [Test]
        public void Calling_not_implemented_method_returns_405()
        {
            var client = new JsonServiceClient(ListeningOn);
            try
            {
                var response = client.Put<MoviesZipResponse>("movies.zip", new MoviesZip());
                Assert.Fail("Should throw 405 excetpion");
            }
            catch (WebServiceException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(405));
            }
        }

        /// <summary>Can get single gethttpresult using rest client with jsonp from service returning HTTP result.</summary>
        [Test]
        public void Can_GET_single_gethttpresult_using_RestClient_with_JSONP_from_service_returning_HttpResult()
        {
            var url = ListeningOn + "gethttpresult?callback=cb";
            string response;

            var webReq = (HttpWebRequest)WebRequest.Create(url);
            webReq.Accept = "*/*";
            using (var webRes = webReq.GetResponse())
            {
                Assert.That(webRes.ContentType, Is.StringStarting(ContentType.Json));
                response = webRes.DownloadText();
            }

            Assert.That(response, Is.Not.Null, "No response received");
            Console.WriteLine(response);
            Assert.That(response, Is.StringStarting("cb("));
            Assert.That(response, Is.StringEnding(")"));
        }

        /// <summary>Debug host.</summary>
        [Test, Ignore]
        public void DebugHost()
        {
            Thread.Sleep(180 * 1000);
        }

        /// <summary>Tests performance.</summary>
        [Test, Ignore]
        public void PerformanceTest()
        {
            const int clientCount = 500;
            var threads = new List<Thread>(clientCount);
            //ThreadPool.SetMinThreads(500, 50);
            //ThreadPool.SetMaxThreads(1000, 50);

            for (int i = 0; i < clientCount; i++)
            {
                threads.Add(new Thread(() =>
                {
                    var html = (ListeningOn + "long_running").GetStringFromUrl();
                }));
            }

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < clientCount; i++)
            {
                threads[i].Start();
            }


            for (int i = 0; i < clientCount; i++)
            {
                threads[i].Join();
            }

            sw.Stop();

            Trace.TraceInformation("Elapsed time for " + clientCount + " requests : " + sw.Elapsed);
        }
    }
}