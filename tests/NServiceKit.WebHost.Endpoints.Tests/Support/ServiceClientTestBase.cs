using System;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;

namespace NServiceKit.WebHost.Endpoints.Tests.Support
{
    /// <summary>A service client test base.</summary>
	public abstract class ServiceClientTestBase : IDisposable
	{
		private const string BaseUrl = "http://127.0.0.1:8083/";

		private AppHostHttpListenerBase appHost;

        /// <summary>Creates the listener.</summary>
        ///
        /// <returns>The new listener.</returns>
		public abstract AppHostHttpListenerBase CreateListener();

        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			appHost = CreateListener();
			appHost.Init();
			appHost.Start(BaseUrl);
		}

        /// <summary>Executes the test fixture tear down action.</summary>
		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			Dispose();
		}

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			if (appHost == null) return;
			appHost.Dispose();
			appHost = null;
		}

        /// <summary>Send this message.</summary>
        ///
        /// <typeparam name="TRes">Type of the resource.</typeparam>
        /// <param name="request"> The request.</param>
        /// <param name="validate">The validate.</param>
		public void Send<TRes>(object request, Action<TRes> validate)
		{
			using (var xmlClient = new XmlServiceClient(BaseUrl))
			using (var jsonClient = new JsonServiceClient(BaseUrl))
			using (var jsvClient = new JsvServiceClient(BaseUrl))
			{
				var xmlResponse = xmlClient.Send<TRes>(request);
				validate(xmlResponse);

				var jsonResponse = jsonClient.Send<TRes>(request);
				validate(jsonResponse);

				var jsvResponse = jsvClient.Send<TRes>(request);
				validate(jsvResponse);
			}
		}
	}
}