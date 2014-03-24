using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;
using NServiceKit.WebHost.Endpoints.Tests.Support.Services;

namespace NServiceKit.WebHost.Endpoints.Tests
{
	[TestFixture]
	public class HttpErrorTests
	{
		private const string ListeningOn = "http://localhost:82/";

		ExampleAppHostHttpListener appHost;

		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
			appHost = new ExampleAppHostHttpListener();
			appHost.Init();
			appHost.Start(ListeningOn);
		}

		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			appHost.Dispose();
		}

		private static void FailOnAsyncError<T>(T response, Exception ex)
		{
			Assert.Fail(ex.Message);
		}

		public IRestClientAsync CreateRestClient()
		{
			return new JsonRestClientAsync();
		}

		[Test]
		public void GET_returns_ArgumentNullException()
		{
			var restClient = CreateRestClient();

			WebServiceException webEx = null;
			HttpErrorResponse response = null;
			restClient.GetAsync<HttpErrorResponse>(ListeningOn + "errors",
				r => response = r,
				(r, ex) => {
					response = r;
					webEx = (WebServiceException)ex;
				});

			Thread.Sleep(1000);

			Assert.That(webEx.StatusCode, Is.EqualTo(400));
			Assert.That(response.ResponseStatus.ErrorCode, Is.EqualTo(typeof(ArgumentNullException).Name));
		}

		[Test]
		public void GET_returns_custom_Exception_and_StatusCode()
		{
			var restClient = CreateRestClient();

			WebServiceException webEx = null;
			HttpErrorResponse response = null;
			restClient.GetAsync<HttpErrorResponse>(ListeningOn + "errors/FileNotFoundException/404",
				r => response = r,
				(r, ex) =>
				{
					response = r;
					webEx = (WebServiceException)ex;
				});

			Thread.Sleep(1000);

			Assert.That(webEx.StatusCode, Is.EqualTo(404));
			Assert.That(response.ResponseStatus.ErrorCode, Is.EqualTo(typeof(FileNotFoundException).Name));
		}

		[Test]
		public void GET_returns_custom_Exception_Message_and_StatusCode()
		{
			var restClient = CreateRestClient();

			WebServiceException webEx = null;
			HttpErrorResponse response = null;
			restClient.GetAsync<HttpErrorResponse>(ListeningOn + "errors/FileNotFoundException/404/ClientErrorMessage",
				r => response = r,
				(r, ex) =>
				{
					response = r;
					webEx = (WebServiceException)ex;
				});

			Thread.Sleep(1000);

			Assert.That(webEx.StatusCode, Is.EqualTo(404));
			Assert.That(response.ResponseStatus.ErrorCode, Is.EqualTo(typeof(FileNotFoundException).Name));
			Assert.That(response.ResponseStatus.Message, Is.EqualTo("ClientErrorMessage"));
		}

	}

}