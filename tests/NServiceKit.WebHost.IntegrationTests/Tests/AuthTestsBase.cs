using System;
using System.Net;
using NUnit.Framework;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>An authentication tests base.</summary>
	public class AuthTestsBase
	{
        /// <summary>URI of the base.</summary>
	    public const string BaseUri = Config.NServiceKitBaseUri;
        /// <summary>The admin email.</summary>
		public const string AdminEmail = "admin@NServiceKit.com";
        /// <summary>The authentication secret.</summary>
	    public const string AuthSecret = "secretz";
		private const string AdminPassword = "E8828A3E26884CE0B345D0D2DFED358A";

		private IServiceClient serviceClient;

        /// <summary>Gets the service client.</summary>
        ///
        /// <value>The service client.</value>
		public IServiceClient ServiceClient
		{
			get
			{
				return serviceClient ?? (serviceClient = new JsonServiceClient(BaseUri));
			}
		}

        /// <summary>Creates admin user.</summary>
        ///
        /// <returns>The new admin user.</returns>
		public Registration CreateAdminUser()
		{
			var registration = new Registration {
				UserName = "Admin",
				DisplayName = "The Admin User",
				Email = AdminEmail, //this email is automatically assigned as Admin in Web.Config
				FirstName = "Admin",
				LastName = "User",
				Password = AdminPassword,
			};
			try
			{
				ServiceClient.Send(registration);
			}
			catch (WebServiceException ex)
			{
				("Error while creating Admin User: " + ex.Message).Print();
				ex.ResponseDto.PrintDump();
			}
			return registration;
		}

        /// <summary>Login.</summary>
        ///
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        ///
        /// <returns>A JsonServiceClient.</returns>
		public JsonServiceClient Login(string userName, string password)
		{
			var client = new JsonServiceClient(BaseUri);
			client.Send(new Auth {
				UserName = userName,
				Password = password,
				RememberMe = true,
			});

			return client;
		}

        /// <summary>Authenticate with admin user.</summary>
        ///
        /// <returns>A JsonServiceClient.</returns>
		public JsonServiceClient AuthenticateWithAdminUser()
		{
			var registration = CreateAdminUser();
			var adminServiceClient = new JsonServiceClient(BaseUri);
			adminServiceClient.Send(new Auth {
				UserName = registration.UserName,
				Password = registration.Password,
				RememberMe = true,
			});

			return adminServiceClient;
		}

        /// <summary>Assert un authorized.</summary>
        ///
        /// <param name="webEx">Details of the exception.</param>
		protected void AssertUnAuthorized(WebServiceException webEx)
		{
			Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
			Assert.That(webEx.StatusDescription, Is.EqualTo(HttpStatusCode.Unauthorized.ToString()));
		}

	}

}