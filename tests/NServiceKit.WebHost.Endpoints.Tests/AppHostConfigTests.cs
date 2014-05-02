using System;
using NUnit.Framework;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>An application host configuration tests.</summary>
	[TestFixture]
	public class AppHostConfigTests
	{
        /// <summary>The listening on.</summary>
		protected const string ListeningOn = "http://localhost:85/";

		TestConfigAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
			LogManager.LogFactory = new ConsoleLogFactory();

			appHost = new TestConfigAppHostHttpListener();
			appHost.Init();
			appHost.Start(ListeningOn);
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

		
        /// <summary>Actually uses the bcl JSON serializers.</summary>
		[Test]
		public void Actually_uses_the_BclJsonSerializers()
		{
			var json = (ListeningOn + "login/user/pass").GetJsonFromUrl();

			Console.WriteLine(json);
			Assert.That(json, Is.EqualTo("{\"pwd\":\"pass\",\"uname\":\"user\"}"));
		}
	}
}