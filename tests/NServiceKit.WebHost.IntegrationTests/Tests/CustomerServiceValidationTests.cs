using System.Collections;
using System.Linq;
using System.Net;
using NUnit.Framework;
using NServiceKit.Service;
using NServiceKit.Text;
using NServiceKit.ServiceClient.Web;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A customer service validation tests.</summary>
	[TestFixture]
	public class CustomerServiceValidationTests
	{
		private const string ListeningOn = Config.NServiceKitBaseUri;

		private string[] ExpectedPostErrorFields = new[] {
			"Id",
			"LastName",
			"FirstName",
			"Company",
			"Address",
			"Postcode",
		};

		private string[] ExpectedPostErrorCodes = new[] {
			"NotEqual",
			"ShouldNotBeEmpty",
			"NotEmpty",
			"NotNull",
			"Length",
			"Predicate",
		};

		Customers validRequest;

        /// <summary>Sets the up.</summary>
		[SetUp]
		public void SetUp()
		{
			validRequest = new Customers {
				Id = 1,
				FirstName = "FirstName",
				LastName = "LastName",
				Address = "12345 Address St, New York",
				Company = "Company",
				Discount = 10,
				HasDiscount = true,
				Postcode = "11215",
			};
		}

        /// <summary>Gets the service clients.</summary>
        ///
        /// <value>The service clients.</value>
		public static IEnumerable ServiceClients
		{
			get
			{
				return new IServiceClient[] {
					new JsonServiceClient(ListeningOn),
					new JsvServiceClient(ListeningOn),
					new XmlServiceClient(ListeningOn),
				};
			}
		}

        /// <summary>Posts an empty request throws validation exception.</summary>
        ///
        /// <exception cref="Validation">Thrown when a validation error condition occurs.</exception>
        ///
        /// <param name="client">The client.</param>
		[Test, TestCaseSource(typeof(CustomerServiceValidationTests), "ServiceClients")]
		public void Post_empty_request_throws_validation_exception(IServiceClient client)
		{
			try
			{
				var response = client.Send(new Customers());
				response.PrintDump();
				Assert.Fail("Should throw Validation Exception");
			}
			catch (WebServiceException ex)
			{
				var response = (CustomersResponse)ex.ResponseDto;

				var errorFields = response.ResponseStatus.Errors;
				var fieldNames = errorFields.Select(x => x.FieldName).ToArray();
				var fieldErrorCodes = errorFields.Select(x => x.ErrorCode).ToArray();

				Assert.That(ex.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
				Assert.That(errorFields.Count, Is.EqualTo(ExpectedPostErrorFields.Length));
				Assert.That(fieldNames, Is.EquivalentTo(ExpectedPostErrorFields));
				Assert.That(fieldErrorCodes, Is.EquivalentTo(ExpectedPostErrorCodes));
			}
		}

        /// <summary>Gets empty request throws validation exception.</summary>
        ///
        /// <exception cref="Validation">Thrown when a validation error condition occurs.</exception>
        ///
        /// <param name="client">The client.</param>
		[Test, TestCaseSource(typeof(CustomerServiceValidationTests), "ServiceClients")]
		public void Get_empty_request_throws_validation_exception(IRestClient client)
		{
			try
			{
				var response = client.Get(new Customers());
				response.PrintDump();
				Assert.Fail("Should throw Validation Exception");
			}
			catch (WebServiceException ex)
			{
				var response = (CustomersResponse)ex.ResponseDto;

				var errorFields = response.ResponseStatus.Errors;
				Assert.That(ex.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
				Assert.That(errorFields.Count, Is.EqualTo(1));
				Assert.That(errorFields[0].ErrorCode, Is.EqualTo("NotEqual"));
				Assert.That(errorFields[0].FieldName, Is.EqualTo("Id"));
			}
		}

        /// <summary>Posts a valid request succeeds.</summary>
        ///
        /// <param name="client">The client.</param>
		[Test, TestCaseSource(typeof(CustomerServiceValidationTests), "ServiceClients")]
		public void Post_ValidRequest_succeeds(IServiceClient client)
		{
			var response = client.Send(validRequest);
			Assert.That(response.ResponseStatus, Is.Null);
		}

	}

}