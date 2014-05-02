using System;
using System.Net;
using System.Threading;
using NUnit.Framework;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;
using System.Linq;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A remote end drops connection tests.</summary>
	[TestFixture]
	public class RemoteEndDropsConnectionTests
	{
		private const string ListeningOn = "http://localhost:82/";
		ExampleAppHostHttpListener appHost;

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.RemoteEndDropsConnectionTests class.</summary>
		public RemoteEndDropsConnectionTests()
		{
			LogManager.LogFactory = new TestLogFactory();
		}

        /// <summary>Executes the test fixture start up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureStartUp()
		{
			appHost = new ExampleAppHostHttpListener();
			LogManager.LogFactory = new TestLogFactory();
			appHost.Init();
			appHost.Start(ListeningOn);

			Console.WriteLine("ExampleAppHost Created at {0}, listening on {1}",
				DateTime.Now, ListeningOn);
		}

        /// <summary>Executes the test fixture tear down action.</summary>
		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			appHost.Dispose();
			appHost = null;
		}

        /// <summary>Sets the up.</summary>
		[SetUp]
		public void SetUp()
		{
			//Clear the logs to get rid of setup messages (registering services)
			TestLogger.GetLogs().Clear();
		}

        /// <summary>Tear down.</summary>
		[TearDown]
		public void TearDown()
		{
			//Clear the logs so other tests dont inherit log entries
			TestLogger.GetLogs().Clear();
		}

		/// <summary>
		/// *Request* DTO
		/// </summary>
        [ServiceHost.Route("/test/timed", "GET")]
		public class Timed
		{
			/// <summary>
			/// Time for the request handler to take before returning.
			/// </summary>
			public int Milliseconds { get; set; }
		}

        /// <summary>A timed service.</summary>
		public class TimedService : ServiceInterface.Service
		{
            /// <summary>Anies the given request.</summary>
            ///
            /// <param name="request">The request.</param>
            ///
            /// <returns>An object.</returns>
            public object Any(Timed request)
			{
				Thread.Sleep(request.Milliseconds);
				return true;
			}
		}

		/// <summary>
		/// This test calls a test service and then aborts the HTTP GET and verifies the host behave in the expected way.
		/// a) when the setting is to write errors to response
		/// b) when the setting is NOT to write errors to response
		/// </summary>
		[Ignore("Hard to know what to change - need to verify this is correct behaviour")]
		[TestCase(false)]
		[TestCase(true)]
		public void TestClientDropsConnection(bool writeErrorsToResponse)
		{
			EndpointHost.Config.WriteErrorsToResponse = writeErrorsToResponse;

			const int sleepMs = 1000;
			var url = string.Format("{0}test/timed?Milliseconds={1}", ListeningOn, sleepMs);
			var req = WebRequest.Create(url) as HttpWebRequest;
			//Set a short timeout so we'll give up before the request is processed
			req.Timeout = 100;
			try
			{
				var res = (HttpWebResponse)req.GetResponse();
			}
			catch (WebException ex)
			{
				if (ex.Status != WebExceptionStatus.Timeout)
					throw;

				//Do nothing - we are expecting a time out
			}

			//Sleep to give the appHost the chance to log the problems so we can investigate
			Thread.Sleep(sleepMs * 2);

			foreach (var pair in TestLogger.GetLogs())
			{
				Console.WriteLine("TEST: {0}: {1}", pair.Key, pair.Value);
			}

			if (!writeErrorsToResponse)
			{
				//Arguably there should be only one Error reported, but we get two. Lets check them both

				//We should get only one log entry: An ERROR from ProcessRequest
				var errorLogs = TestLogger.GetLogs().Where(o => o.Key == TestLogger.Levels.ERROR).ToList();
				Assert.AreEqual(2, errorLogs.Count, "Checking if there is only one ERROR entry");

				StringAssert.Contains("Error in HttpListenerResponseWrapper", errorLogs[0].Value, "Checking if the error is from HttpListenerResponseWrapper");
				StringAssert.Contains("ProcessRequest", errorLogs[1].Value, "Checking if the error is from ProcessRequest");
			}
			else
			{
				//There is quite a lot of logging going on here and arguably there should be only one Error reported.
			}
		}
	}

}