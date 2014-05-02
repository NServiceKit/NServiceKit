using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Common.Tests.OAuth
{
    /// <summary>The credentials service tests.</summary>
	[TestFixture]
	public class CredentialsServiceTests
	{
        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			AuthService.Init(() => new AuthUserSession(),
				new CredentialsAuthProvider());           
		}

        /// <summary>Gets authentication service.</summary>
        ///
        /// <returns>The authentication service.</returns>
		public AuthService GetAuthService()
		{
		    var authService = new AuthService {
                RequestContext = new MockRequestContext(),
                //ServiceExceptionHandler = (req, ex) =>
                //    ValidationFeature.HandleException(new BasicResolver(), req, ex)
            };
		    return authService;
		}

        class ValidateServiceRunner<T> : ServiceRunner<T>
        {
            /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.OAuth.CredentialsServiceTests.ValidateServiceRunner&lt;T&gt; class.</summary>
            ///
            /// <param name="appHost">      The application host.</param>
            /// <param name="actionContext">Context for the action.</param>
            public ValidateServiceRunner(IAppHost appHost, ActionContext actionContext)
                : base(appHost, actionContext) {}

            /// <summary>Handles the exception.</summary>
            ///
            /// <param name="requestContext">Context for the request.</param>
            /// <param name="request">       The request.</param>
            /// <param name="ex">            The ex.</param>
            ///
            /// <returns>An object.</returns>
            public override object HandleException(IRequestContext requestContext, T request, System.Exception ex)
            {
                return DtoUtils.HandleException(new BasicResolver(), request, ex);
            }
        }

        /// <summary>Gets authentication service.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="request">    The request.</param>
        ///
        /// <returns>The authentication service.</returns>
        public object GetAuthService(AuthService authService, Auth request)
        {
            var serviceRunner = new ValidateServiceRunner<Auth>(null, new ActionContext {
                Id = "GET Auth",
                ServiceAction = (service, req) => ((AuthService)service).Get((Auth)req)
            });

            return serviceRunner.Process(authService.RequestContext, authService, request);
        }

        /// <summary>Empty request invalidates all fields.</summary>
	    [Test]
		public void Empty_request_invalidates_all_fields()
		{
			var authService = GetAuthService();

            var response = (HttpError)GetAuthService(authService, new Auth());
			var errors = response.GetFieldErrors();

			Assert.That(errors.Count, Is.EqualTo(2));
			Assert.That(errors[0].ErrorCode, Is.EqualTo("NotEmpty"));
			Assert.That(errors[0].FieldName, Is.EqualTo("UserName"));
			Assert.That(errors[1].ErrorCode, Is.EqualTo("NotEmpty"));
			Assert.That(errors[1].FieldName, Is.EqualTo("Password"));
		}

        /// <summary>Requires user name and password.</summary>
		[Test]
		public void Requires_UserName_and_Password()
		{
			var authService = GetAuthService();

            var response = (HttpError)GetAuthService(authService,
                new Auth { provider = AuthService.CredentialsProvider });

			var errors = response.GetFieldErrors();

			Assert.That(errors.Count, Is.EqualTo(2));
			Assert.That(errors[0].ErrorCode, Is.EqualTo("NotEmpty"));
			Assert.That(errors[0].FieldName, Is.EqualTo("UserName"));
			Assert.That(errors[1].FieldName, Is.EqualTo("Password"));
			Assert.That(errors[1].ErrorCode, Is.EqualTo("NotEmpty"));
		}
	}
}