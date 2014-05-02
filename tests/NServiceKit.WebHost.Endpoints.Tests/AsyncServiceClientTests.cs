using System;
using System.Threading;
using NUnit.Framework;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>The asynchronous service client tests.</summary>
	public abstract class AsyncServiceClientTests
	{
        /// <summary>The listening on.</summary>
		protected const string ListeningOn = "http://localhost:82/";

		ExampleAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
			appHost = new ExampleAppHostHttpListener();
			appHost.Init();
			appHost.Start(ListeningOn);
		}

        /// <summary>Executes the test fixture tear down action.</summary>
		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			appHost.Dispose();
		}

        /// <summary>Creates service client.</summary>
        ///
        /// <returns>The new service client.</returns>
		protected abstract IServiceClient CreateServiceClient();

		private static void FailOnAsyncError<T>(T response, Exception ex)
		{
			Assert.Fail(ex.Message);
		}

        /// <summary>Can call send asynchronous on service client.</summary>
		[Test]
		public void Can_call_SendAsync_on_ServiceClient()
		{
			var jsonClient = new JsonServiceClient(ListeningOn);

			var request = new GetFactorial { ForNumber = 3 };
			GetFactorialResponse response = null;
			jsonClient.SendAsync<GetFactorialResponse>(request, r => response = r, FailOnAsyncError);

			Thread.Sleep(1000);

			Assert.That(response, Is.Not.Null, "No response received");
			Assert.That(response.Result, Is.EqualTo(GetFactorialService.GetFactorial(request.ForNumber)));
		}

        /// <summary>A JSON asynchronous service client tests.</summary>
		[TestFixture]
		public class JsonAsyncServiceClientTests : AsyncServiceClientTests
		{
            /// <summary>Creates service client.</summary>
            ///
            /// <returns>The new service client.</returns>
			protected override IServiceClient CreateServiceClient()
			{
				return new JsonServiceClient(ListeningOn);
			}
		}

        /// <summary>A jsv asynchronous service client tests.</summary>
		[TestFixture]
		public class JsvAsyncServiceClientTests : AsyncServiceClientTests
		{
            /// <summary>Creates service client.</summary>
            ///
            /// <returns>The new service client.</returns>
			protected override IServiceClient CreateServiceClient()
			{
				return new JsvServiceClient(ListeningOn);
			}
		}

        /// <summary>An XML asynchronous service client tests.</summary>
		[TestFixture]
		public class XmlAsyncServiceClientTests : AsyncServiceClientTests
		{
            /// <summary>Creates service client.</summary>
            ///
            /// <returns>The new service client.</returns>
			protected override IServiceClient CreateServiceClient()
			{
				return new XmlServiceClient(ListeningOn);
			}
		}
	}


}