using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Funq;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.ServiceClient.Web;
using NServiceKit.FluentValidation;
using NServiceKit.Service;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.ServiceInterface.Validation;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Support;
using NServiceKit.WebHost.Endpoints.Tests;
using NServiceKit.WebHost.Endpoints.Tests.Support;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A customers.</summary>
	[Route("/customers")]
	[Route("/customers/{Id}")]
	public class Customers
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
		public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
		public string LastName { get; set; }

        /// <summary>Gets or sets the company.</summary>
        ///
        /// <value>The company.</value>
		public string Company { get; set; }

        /// <summary>Gets or sets the discount.</summary>
        ///
        /// <value>The discount.</value>
		public decimal Discount { get; set; }

        /// <summary>Gets or sets the address.</summary>
        ///
        /// <value>The address.</value>
		public string Address { get; set; }

        /// <summary>Gets or sets the postcode.</summary>
        ///
        /// <value>The postcode.</value>
		public string Postcode { get; set; }

        /// <summary>Gets or sets a value indicating whether this object has discount.</summary>
        ///
        /// <value>true if this object has discount, false if not.</value>
		public bool HasDiscount { get; set; }
	}

    /// <summary>Interface for address validator.</summary>
	public interface IAddressValidator
	{
        /// <summary>Valid address.</summary>
        ///
        /// <param name="address">The address.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool ValidAddress(string address);
	}

    /// <summary>The address validator.</summary>
	public class AddressValidator : IAddressValidator
	{
        /// <summary>Valid address.</summary>
        ///
        /// <param name="address">The address.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool ValidAddress(string address)
		{
			return address != null
				&& address.Length >= 20
				&& address.Length <= 250;
		}
	}

    /// <summary>The customers validator.</summary>
	public class CustomersValidator : AbstractValidator<Customers>
	{
        /// <summary>Gets or sets the address validator.</summary>
        ///
        /// <value>The address validator.</value>
		public IAddressValidator AddressValidator { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.CustomersValidator class.</summary>
		public CustomersValidator()
		{
			RuleFor(x => x.Id).NotEqual(default(int));

			RuleSet(ApplyTo.Post | ApplyTo.Put, () => {
				RuleFor(x => x.LastName).NotEmpty().WithErrorCode("ShouldNotBeEmpty");
				RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please specify a first name");
				RuleFor(x => x.Company).NotNull();
				RuleFor(x => x.Discount).NotEqual(0).When(x => x.HasDiscount);
				RuleFor(x => x.Address).Must(x => AddressValidator.ValidAddress(x));
				RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
			});
		}

		static readonly Regex UsPostCodeRegEx = new Regex(@"^\d{5}(-\d{4})?$", RegexOptions.Compiled);

		private bool BeAValidPostcode(string postcode)
		{
			return !string.IsNullOrEmpty(postcode) && UsPostCodeRegEx.IsMatch(postcode);
		}
	}

    /// <summary>The customers response.</summary>
	public class CustomersResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		public Customers Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A customer service.</summary>
    [DefaultRequest(typeof(Customers))]
	public class CustomerService : ServiceInterface.Service
	{
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(Customers request)
		{
			return new CustomersResponse { Result = request };
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(Customers request)
		{
			return new CustomersResponse { Result = request };
		}

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Put(Customers request)
		{
			return new CustomersResponse { Result = request };
		}

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Delete(Customers request)
		{
			return new CustomersResponse { Result = request };
		}
	}

    /// <summary>A customer service validation tests.</summary>
	[TestFixture]
	public class CustomerServiceValidationTests
	{
		private const string ListeningOn = "http://localhost:82/";

        /// <summary>A validation application host HTTP listener.</summary>
		public class ValidationAppHostHttpListener
			: AppHostHttpListenerBase
		{
            /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.CustomerServiceValidationTests.ValidationAppHostHttpListener class.</summary>
			public ValidationAppHostHttpListener()
				: base("Validation Tests", typeof(CustomerService).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
			public override void Configure(Container container)
			{
				Plugins.Add(new ValidationFeature());
				container.Register<IAddressValidator>(new AddressValidator());
				container.RegisterValidators(typeof(CustomersValidator).Assembly);
			}
		}

		ValidationAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
			appHost = new ValidationAppHostHttpListener();
			appHost.Init();
			appHost.Start(ListeningOn);
		}

        /// <summary>Executes the test fixture tear down action.</summary>
		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			appHost.Dispose();
		    EndpointHandlerBase.ServiceManager = null;
		}

		private static List<ResponseError> GetValidationFieldErrors(string httpMethod, Customers request)
		{
			var validator = (IValidator)new CustomersValidator {
				AddressValidator = new AddressValidator()
			};

			var validationResult = validator.Validate(
			new ValidationContext(request, null, new MultiRuleSetValidatorSelector(httpMethod)));

            var responseStatus = validationResult.ToErrorResult().ToResponseStatus();

			var errorFields = responseStatus.Errors;
			return errorFields ?? new List<ResponseError>();
		}

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
			"Predicate",
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

        /// <summary>Validation feature add request filter once.</summary>
        [Test]
        public void ValidationFeature_add_request_filter_once()
        {
            var old = appHost.RequestFilters.Count; 
            appHost.LoadPlugin(new ValidationFeature());
            Assert.That(old, Is.EqualTo(appHost.RequestFilters.Count));
        }
		
        /// <summary>Validates valid request on post.</summary>
		[Test]
		public void Validates_ValidRequest_request_on_Post()
		{
			var errorFields = GetValidationFieldErrors(HttpMethods.Post, validRequest);
			Assert.That(errorFields.Count, Is.EqualTo(0));
		}

        /// <summary>Validates valid request on get.</summary>
		[Test]
		public void Validates_ValidRequest_request_on_Get()
		{
			var errorFields = GetValidationFieldErrors(HttpMethods.Get, validRequest);
			Assert.That(errorFields.Count, Is.EqualTo(0));
		}

        /// <summary>Validates conditional request on post.</summary>
		[Test]
		public void Validates_Conditional_Request_request_on_Post()
		{
			validRequest.Discount = 0;
			validRequest.HasDiscount = true;

			var errorFields = GetValidationFieldErrors(HttpMethods.Post, validRequest);
			Assert.That(errorFields.Count, Is.EqualTo(1));
			Assert.That(errorFields[0].FieldName, Is.EqualTo("Discount"));
		}

        /// <summary>Validates empty request on post.</summary>
		[Test]
		public void Validates_empty_request_on_Post()
		{
			var request = new Customers();
			var errorFields = GetValidationFieldErrors(HttpMethods.Post, request);

			var fieldNames = errorFields.Select(x => x.FieldName).ToArray();
			var fieldErrorCodes = errorFields.Select(x => x.ErrorCode).ToArray();

			Assert.That(errorFields.Count, Is.EqualTo(ExpectedPostErrorFields.Length));
			Assert.That(fieldNames, Is.EquivalentTo(ExpectedPostErrorFields));
			Assert.That(fieldErrorCodes, Is.EquivalentTo(ExpectedPostErrorCodes));
		}

        /// <summary>Validates empty request on put.</summary>
		[Test]
		public void Validates_empty_request_on_Put()
		{
			var request = new Customers();
			var errorFields = GetValidationFieldErrors(HttpMethods.Put, request);

			var fieldNames = errorFields.Select(x => x.FieldName).ToArray();
			var fieldErrorCodes = errorFields.Select(x => x.ErrorCode).ToArray();

			Assert.That(errorFields.Count, Is.EqualTo(ExpectedPostErrorFields.Length));
			Assert.That(fieldNames, Is.EquivalentTo(ExpectedPostErrorFields));
			Assert.That(fieldErrorCodes, Is.EquivalentTo(ExpectedPostErrorCodes));
		}

        /// <summary>Validates empty request on get.</summary>
		[Test]
		public void Validates_empty_request_on_Get()
		{
			var request = new Customers();
			var errorFields = GetValidationFieldErrors(HttpMethods.Get, request);

			Assert.That(errorFields.Count, Is.EqualTo(1));
			Assert.That(errorFields[0].ErrorCode, Is.EqualTo("NotEqual"));
			Assert.That(errorFields[0].FieldName, Is.EqualTo("Id"));
		}

        /// <summary>Validates empty request on delete.</summary>
		[Test]
		public void Validates_empty_request_on_Delete()
		{
			var request = new Customers();
			var errorFields = GetValidationFieldErrors(HttpMethods.Delete, request);

			Assert.That(errorFields.Count, Is.EqualTo(1));
			Assert.That(errorFields[0].ErrorCode, Is.EqualTo("NotEqual"));
			Assert.That(errorFields[0].FieldName, Is.EqualTo("Id"));
		}

        /// <summary>Unit test service client.</summary>
        ///
        /// <returns>An IServiceClient.</returns>
		protected static IServiceClient UnitTestServiceClient()
		{
            EndpointHandlerBase.ServiceManager = new ServiceManager(typeof(SecureService).Assembly).Init();
			return new DirectServiceClient(EndpointHandlerBase.ServiceManager);
		}

        /// <summary>Gets the service clients.</summary>
        ///
        /// <value>The service clients.</value>
		public static IEnumerable ServiceClients
		{
			get
			{
				//Seriously retarded workaround for some devs idea who thought this should
				//be run for all test fixtures, not just this one.

				return new Func<IServiceClient>[] {
					() => UnitTestServiceClient(),
					() => new JsonServiceClient(ListeningOn),
					() => new JsvServiceClient(ListeningOn),
					() => new XmlServiceClient(ListeningOn),
				};
			}
		}

        /// <summary>Posts an empty request throws validation exception.</summary>
        ///
        /// <exception cref="Validation">Thrown when a validation error condition occurs.</exception>
        ///
        /// <param name="factory">The factory.</param>
		[Test, TestCaseSource(typeof(CustomerServiceValidationTests), "ServiceClients")]
		public void Post_empty_request_throws_validation_exception(Func<IServiceClient> factory)
		{
			try
			{
				var client = factory();
				var response = client.Send<CustomersResponse>(new Customers());
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
        /// <param name="factory">The factory.</param>
		[Test, TestCaseSource(typeof(CustomerServiceValidationTests), "ServiceClients")]
		public void Get_empty_request_throws_validation_exception(Func<IServiceClient> factory)
		{
			try
			{
				var client = (IRestClient)factory();
				var response = client.Get<CustomersResponse>("Customers");
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
        /// <param name="factory">The factory.</param>
		[Test, TestCaseSource(typeof(CustomerServiceValidationTests), "ServiceClients")]
		public void Post_ValidRequest_succeeds(Func<IServiceClient> factory)
		{
			var client = factory();
			var response = client.Send<CustomersResponse>(validRequest);
			Assert.That(response.ResponseStatus, Is.Null);
		}

	}
}