using System;
using System.Net;
using NUnit.Framework;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Support;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
	/// <summary>
	/// This base class executes all Web Services ignorant of the endpoints its hosted on.
	/// The same tests below are re-used by the Unit and Integration TestFixture's declared below
	/// </summary>
	[TestFixture]
	public abstract class WebServicesTests
		: TestBase
	{
		private const string TestString = "NServiceKit";

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Tests.WebServicesTests class.</summary>
		protected WebServicesTests()
			: base(Config.NServiceKitBaseUri, typeof(ReverseService).Assembly)
		{
		}

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
		protected override void Configure(Funq.Container container) { }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            EndpointHandlerBase.ServiceManager = null;
        }

        /// <summary>Does execute reverse service.</summary>
		[Test]
		public void Does_Execute_ReverseService()
		{
			var client = CreateNewServiceClient();
			var response = client.Send<ReverseResponse>(
				new Reverse { Value = TestString });

			var expectedValue = ReverseService.Execute(TestString);
			Assert.That(response.Result, Is.EqualTo(expectedValue));
		}

        /// <summary>Does execute rot 13 service.</summary>
		[Test]
		public void Does_Execute_Rot13Service()
		{
			var client = CreateNewServiceClient();
			var response = client.Send<Rot13Response>(new Rot13 { Value = TestString });

			var expectedValue = TestString.ToRot13();
			Assert.That(response.Result, Is.EqualTo(expectedValue));
		}

        /// <summary>Can handle exception from always throw service.</summary>
        ///
        /// <exception cref="HTTP">Thrown when a HTTP error condition occurs.</exception>
		[Test]
		public void Can_Handle_Exception_from_AlwaysThrowService()
		{
			var client = CreateNewServiceClient();
			try
			{
				var response = client.Send<AlwaysThrowsResponse>(
					new AlwaysThrows { Value = TestString });

				response.PrintDump();
				Assert.Fail("Should throw HTTP errors");
			}
			catch (WebServiceException webEx)
			{
				var response = (AlwaysThrowsResponse) webEx.ResponseDto;
				var expectedError = AlwaysThrowsService.GetErrorMessage(TestString);
				Assert.That(response.ResponseStatus.ErrorCode,
					Is.EqualTo(typeof(NotImplementedException).Name));
				Assert.That(response.ResponseStatus.Message,
					Is.EqualTo(expectedError));
			}
		}

        /// <summary>Request items are preserved between filters.</summary>
        [Test]
        public void Request_items_are_preserved_between_filters()
        {
            var client = CreateNewServiceClient();
            if (client is DirectServiceClient) return;
            var response = client.Send<RequestItemsResponse>(new RequestItems { });
            Assert.That(response.Result, Is.EqualTo("MissionSuccess"));
        }
    }


	/// <summary>
	/// Unit tests simulates an in-process NServiceKit host where all services 
	/// are executed all in-memory so DTO's are not even serialized.
	/// </summary>
	public class UnitTests : WebServicesTests
	{
        /// <summary>Creates new service client.</summary>
        ///
        /// <returns>The new new service client.</returns>
		protected override IServiceClient CreateNewServiceClient()
		{
            EndpointHandlerBase.ServiceManager = new ServiceManager(typeof(ReverseService).Assembly).Init();
			return new DirectServiceClient(this, EndpointHandlerBase.ServiceManager);
		}
	}

    /// <summary>An XML integration tests.</summary>
	public class XmlIntegrationTests : WebServicesTests
	{
        /// <summary>Creates new service client.</summary>
        ///
        /// <returns>The new new service client.</returns>
		protected override IServiceClient CreateNewServiceClient()
		{
			return new XmlServiceClient(ServiceClientBaseUri);
		}
	}

    /// <summary>A JSON integration tests.</summary>
	public class JsonIntegrationTests : WebServicesTests
	{
        /// <summary>Creates new service client.</summary>
        ///
        /// <returns>The new new service client.</returns>
		protected override IServiceClient CreateNewServiceClient()
		{
			return new JsonServiceClient(ServiceClientBaseUri);
		}
	}

    /// <summary>A jsv integration tests.</summary>
	public class JsvIntegrationTests : WebServicesTests
	{
        /// <summary>Creates new service client.</summary>
        ///
        /// <returns>The new new service client.</returns>
		protected override IServiceClient CreateNewServiceClient()
		{
			return new JsvServiceClient(ServiceClientBaseUri);
		}
	}

    /// <summary>A SOAP 11 integration tests.</summary>
	public class Soap11IntegrationTests : WebServicesTests
	{
        /// <summary>Creates new service client.</summary>
        ///
        /// <returns>The new new service client.</returns>
		protected override IServiceClient CreateNewServiceClient()
		{
			return new Soap11ServiceClient(ServiceClientBaseUri);
		}
	}

    /// <summary>A SOAP 12 integration tests.</summary>
	public class Soap12IntegrationTests : WebServicesTests
	{
        /// <summary>Creates new service client.</summary>
        ///
        /// <returns>The new new service client.</returns>
		protected override IServiceClient CreateNewServiceClient()
		{
			return new Soap12ServiceClient(ServiceClientBaseUri);
		}
	}

}