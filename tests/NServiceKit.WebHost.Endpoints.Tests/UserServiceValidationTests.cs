using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NServiceKit.FluentValidation;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NUnit.Framework;
using NServiceKit.ServiceInterface.Validation;
using System.Collections;
using Funq;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Service;
using NServiceKit.WebHost.Endpoints.Support;
using NServiceKit.WebHost.Endpoints.Tests.Support;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A user validation.</summary>
    [Route("/uservalidation")]
	[Route("/uservalidation/{Id}")]
    public class UserValidation
    {
        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        public string LastName { get; set; }
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

    /// <summary>A user validator.</summary>
    public class UserValidator : AbstractValidator<UserValidation>, IRequiresHttpRequest
    {
        /// <summary>Gets or sets the address validator.</summary>
        ///
        /// <value>The address validator.</value>
        public IAddressValidator AddressValidator { get; set; }

        /// <summary>Gets or sets the HTTP request.</summary>
        ///
        /// <value>The HTTP request.</value>
		public IHttpRequest HttpRequest { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.UserValidator class.</summary>
        public UserValidator()
        {
			RuleFor(x => x.FirstName).Must(f =>
			{
				if (HttpRequest == null)
					Assert.Fail();

				return true;
			});
			RuleFor(x => x.LastName).NotEmpty().WithErrorCode("ShouldNotBeEmpty");
			RuleSet(ApplyTo.Post | ApplyTo.Put, () =>
            {
                RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please specify a first name");
            });
        }
	}

    //Not matching the naming convention ([Request DTO Name] + "Response")
    public class OperationResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public UserValidation Result { get; set; }
    }

    /// <summary>A user validation service.</summary>
    public class UserValidationService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(UserValidation request)
        {
            return new OperationResponse { Result = request };
        }
    }

    /// <summary>A user service validation tests.</summary>
    [TestFixture]
    public class UserServiceValidationTests
    {
        private const string ListeningOn = "http://localhost:82/";

        /// <summary>A user application host HTTP listener.</summary>
        public class UserAppHostHttpListener
            : AppHostHttpListenerBase
        {

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.UserServiceValidationTests.UserAppHostHttpListener class.</summary>
            public UserAppHostHttpListener()
                : base("Validation Tests", typeof(UserValidationService).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Container container)
            {
				Plugins.Add(new ValidationFeature());
				container.RegisterValidators(typeof(UserValidator).Assembly);
            }
        }

        UserAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            appHost = new UserAppHostHttpListener();
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

        private static string ExpectedErrorCode = "ShouldNotBeEmpty";

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

        /// <summary>Gets empty request throws validation exception.</summary>
        ///
        /// <exception cref="Validation">Thrown when a validation error condition occurs.</exception>
        ///
        /// <param name="factory">The factory.</param>
        [Test, TestCaseSource(typeof(UserServiceValidationTests), "ServiceClients")]
        public void Get_empty_request_throws_validation_exception(Func<IServiceClient> factory)
        {
            try
            {
                var client = (IRestClient)factory();
				var response = client.Get<OperationResponse>("UserValidation");
                Assert.Fail("Should throw Validation Exception");
            }
            catch (WebServiceException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            	Assert.That(ex.StatusDescription, Is.EqualTo(ExpectedErrorCode));
            }
        }

        /// <summary>Gets the rest clients.</summary>
        ///
        /// <value>The rest clients.</value>
        public static IEnumerable RestClients
        {
            get
            {
                //Seriously retarded workaround for some devs idea who thought this should
                //be run for all test fixtures, not just this one.

                return new Func<IServiceClient>[] {
					() => new JsonServiceClient(ListeningOn),
					() => new JsvServiceClient(ListeningOn),
					() => new XmlServiceClient(ListeningOn),
				};
            }
        }

        /// <summary>Throws validation exception even if always send basic authentication header is false.</summary>
        ///
        /// <exception cref="Validation">Thrown when a validation error condition occurs.</exception>
        ///
        /// <param name="factory">The factory.</param>
        [Test, TestCaseSource(typeof(UserServiceValidationTests), "RestClients")]
        public void Throws_validation_exception_even_if_AlwaysSendBasicAuthHeader_is_false(Func<IServiceClient> factory)
        {
            try
            {
                var client = (ServiceClientBase)factory();
                client.AlwaysSendBasicAuthHeader = false;
                var response = client.Get<OperationResponse>("UserValidation");
                Assert.Fail("Should throw Validation Exception");
            }
            catch (WebServiceException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
                Assert.That(ex.StatusDescription, Is.EqualTo(ExpectedErrorCode));
            }
        }
    }
}
