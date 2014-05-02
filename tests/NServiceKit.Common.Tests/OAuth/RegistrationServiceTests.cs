using Moq;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.FluentValidation;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Common.Tests.OAuth
{
    /// <summary>A registration service tests.</summary>
	[TestFixture]
	public class RegistrationServiceTests
	{
        static BasicAppHost _appHost = null;
        static readonly AuthUserSession authUserSession = new AuthUserSession();

        static IAppHost GetAppHost()
        {
            if (_appHost == null)
            {
                _appHost = new BasicAppHost();
                var authService = new AuthService();
                authService.SetResolver(_appHost);
                _appHost.Container.Register(authService);
                _appHost.Container.Register<IAuthSession>(authUserSession);
            }
            return _appHost;
        }
        
        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
            AuthService.Init(() => authUserSession, new CredentialsAuthProvider());            
		}

        /// <summary>Gets stub repo.</summary>
        ///
        /// <returns>The stub repo.</returns>
		public static IUserAuthRepository GetStubRepo()
		{
			var mock = new Mock<IUserAuthRepository>();
			mock.Expect(x => x.GetUserAuthByUserName(It.IsAny<string>()))
				.Returns((UserAuth)null);
			mock.Expect(x => x.CreateUserAuth(It.IsAny<UserAuth>(), It.IsAny<string>()))
				.Returns(new UserAuth { Id = 1 });

			return mock.Object;
		}

        /// <summary>Gets registration service.</summary>
        ///
        /// <param name="validator">  The validator.</param>
        /// <param name="authRepo">   The authentication repo.</param>
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>The registration service.</returns>
		public static RegistrationService GetRegistrationService(
			AbstractValidator<Registration> validator = null, 
			IUserAuthRepository authRepo=null,
			string contentType=null)
		{
			var requestContext = new MockRequestContext();
			if (contentType != null)
			{
				requestContext.ResponseContentType = contentType;
			}
		    var userAuthRepository = authRepo ?? GetStubRepo();
		    var service = new RegistrationService {
                RegistrationValidator = validator ?? new RegistrationValidator { UserAuthRepo = userAuthRepository },
				UserAuthRepo = userAuthRepository,
				RequestContext = requestContext,
			};

		    var appHost = GetAppHost();
            appHost.Register(userAuthRepository);
		    service.SetResolver(appHost);

            return service;
		}

        /// <summary>Empty registration is invalid.</summary>
	    [Test]
		public void Empty_Registration_is_invalid()
		{
			var service = GetRegistrationService();

	        var response = PostRegistrationError(service, new Registration());
			var errors = response.GetFieldErrors();

			Assert.That(errors.Count, Is.EqualTo(3));
			Assert.That(errors[0].ErrorCode, Is.EqualTo("NotEmpty"));
			Assert.That(errors[0].FieldName, Is.EqualTo("Password"));
			Assert.That(errors[1].ErrorCode, Is.EqualTo("NotEmpty"));
			Assert.That(errors[1].FieldName, Is.EqualTo("UserName"));
			Assert.That(errors[2].ErrorCode, Is.EqualTo("NotEmpty"));
			Assert.That(errors[2].FieldName, Is.EqualTo("Email"));
		}

	    private static HttpError PostRegistrationError(RegistrationService service, Registration registration)
	    {
	        var response = (HttpError) service.RunAction(registration, (svc, req) => svc.Post(req));
	        return response;
	    }

        /// <summary>Empty registration is invalid with full registration validator.</summary>
	    [Test]
		public void Empty_Registration_is_invalid_with_FullRegistrationValidator()
		{
			var service = GetRegistrationService(new FullRegistrationValidator());

            var response = PostRegistrationError(service, new Registration());
            var errors = response.GetFieldErrors();

			Assert.That(errors.Count, Is.EqualTo(4));
			Assert.That(errors[0].ErrorCode, Is.EqualTo("NotEmpty"));
			Assert.That(errors[0].FieldName, Is.EqualTo("Password"));
			Assert.That(errors[1].ErrorCode, Is.EqualTo("NotEmpty"));
			Assert.That(errors[1].FieldName, Is.EqualTo("UserName"));
			Assert.That(errors[2].ErrorCode, Is.EqualTo("NotEmpty"));
			Assert.That(errors[2].FieldName, Is.EqualTo("Email"));
			Assert.That(errors[3].ErrorCode, Is.EqualTo("NotEmpty"));
			Assert.That(errors[3].FieldName, Is.EqualTo("DisplayName"));
		}

        /// <summary>Accepts valid registration.</summary>
		[Test]
		public void Accepts_valid_registration()
		{
			var service = GetRegistrationService();

			var request = GetValidRegistration();

			var response = service.Post(request);

			Assert.That(response as RegistrationResponse, Is.Not.Null);
		}

        /// <summary>Gets valid registration.</summary>
        ///
        /// <param name="autoLogin">true to automatically login.</param>
        ///
        /// <returns>The valid registration.</returns>
		public static Registration GetValidRegistration(bool autoLogin=false)
		{
			var request = new Registration {
				DisplayName = "DisplayName",
				Email = "my@email.com",
				FirstName = "FirstName",
				LastName = "LastName",
				Password = "Password",
				UserName = "UserName",
				AutoLogin = autoLogin,
			};
			return request;
		}

        /// <summary>Requires unique user name and email.</summary>
		[Test]
		public void Requires_unique_UserName_and_Email()
		{
			var mock = new Mock<IUserAuthRepository>();
			var mockExistingUser = new UserAuth();
			mock.Expect(x => x.GetUserAuthByUserName(It.IsAny<string>()))
				.Returns(mockExistingUser);

			var service = new RegistrationService {
				RegistrationValidator = new RegistrationValidator { UserAuthRepo = mock.Object },
				UserAuthRepo = mock.Object,
            };

			var request = new Registration {
				DisplayName = "DisplayName",
				Email = "my@email.com",
				FirstName = "FirstName",
				LastName = "LastName",
				Password = "Password",
				UserName = "UserName",
			};

            var response = PostRegistrationError(service, request);
            var errors = response.GetFieldErrors();

			Assert.That(errors.Count, Is.EqualTo(2));
			Assert.That(errors[0].ErrorCode, Is.EqualTo("AlreadyExists"));
			Assert.That(errors[0].FieldName, Is.EqualTo("UserName"));
			Assert.That(errors[1].ErrorCode, Is.EqualTo("AlreadyExists"));
			Assert.That(errors[1].FieldName, Is.EqualTo("Email"));
		}

        /// <summary>Registration with HTML content type and continue returns 302 with location.</summary>
		[Test]
		public void Registration_with_Html_ContentType_And_Continue_returns_302_with_Location()
		{
			var service = GetRegistrationService(null, null, ContentType.Html);

			var request = GetValidRegistration();
			request.Continue = "http://localhost/home";

			var response = service.Post(request) as HttpResult;

			Assert.That(response, Is.Not.Null);
			Assert.That(response.Status, Is.EqualTo(302));
			Assert.That(response.Headers[HttpHeaders.Location], Is.EqualTo("http://localhost/home"));
		}

        /// <summary>Registration with empty string continue returns registration response.</summary>
		[Test]
		public void Registration_with_EmptyString_Continue_returns_RegistrationResponse()
		{
			var service = GetRegistrationService(null, null, ContentType.Html);

			var request = GetValidRegistration();
			request.Continue = string.Empty;

			var response = service.Post(request);

			Assert.That(response as HttpResult, Is.Null);
			Assert.That(response as RegistrationResponse, Is.Not.Null);
		}

        /// <summary>Registration with JSON content type and continue returns registration response with referrer URL.</summary>
		[Test]
		public void Registration_with_Json_ContentType_And_Continue_returns_RegistrationResponse_with_ReferrerUrl()
		{
			var service = GetRegistrationService(null, null, ContentType.Json);

			var request = GetValidRegistration();
			request.Continue = "http://localhost/home";

			var response = service.Post(request);

			Assert.That(response as HttpResult, Is.Null);
			Assert.That(response as RegistrationResponse, Is.Not.Null);
			Assert.That(((RegistrationResponse)response).ReferrerUrl, Is.EqualTo("http://localhost/home"));
		}
	}
}