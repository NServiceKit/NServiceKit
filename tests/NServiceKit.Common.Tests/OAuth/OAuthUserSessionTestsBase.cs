using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.Configuration;
using NServiceKit.OrmLite;
using NServiceKit.OrmLite.SqlServer;
using NServiceKit.Redis;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.ServiceInterface.Testing;

namespace NServiceKit.Common.Tests.OAuth
{
    /// <summary>An authentication user session tests base.</summary>
	public abstract class OAuthUserSessionTestsBase
	{
        /// <summary>The load user authentication repositorys.</summary>
		public static bool LoadUserAuthRepositorys = true;

		//Can only use either 1 OrmLiteDialectProvider at 1-time SqlServer or Sqlite.
		public static bool UseSqlServer = false;

        /// <summary>Gets new session 2.</summary>
        ///
        /// <returns>The new session 2.</returns>
		public static AuthUserSession GetNewSession2()
		{
			var oAuthUserSession = new AuthUserSession();
			return oAuthUserSession;
		}

        /// <summary>Gets credentials authentication configuration.</summary>
        ///
        /// <returns>The credentials authentication configuration.</returns>
		public CredentialsAuthProvider GetCredentialsAuthConfig()
		{
			return new CredentialsAuthProvider(new AppSettings()) {
			};
		}

        /// <summary>Gets twitter authentication provider.</summary>
        ///
        /// <returns>The twitter authentication provider.</returns>
		public TwitterAuthProvider GetTwitterAuthProvider()
		{
			return new TwitterAuthProvider(new AppSettings()) {
				AuthHttpGateway = new MockAuthHttpGateway(),
			};
		}

        /// <summary>Gets facebook authentication provider.</summary>
        ///
        /// <returns>The facebook authentication provider.</returns>
		public FacebookAuthProvider GetFacebookAuthProvider()
		{
			return new FacebookAuthProvider(new AppSettings()) {
				AuthHttpGateway = new MockAuthHttpGateway(),
			};
		}

        /// <summary>Gets the user authentication repositorys.</summary>
        ///
        /// <value>The user authentication repositorys.</value>
		public IEnumerable UserAuthRepositorys
		{
			get
			{
				if (!LoadUserAuthRepositorys) yield break;

				var inMemoryRepo = new InMemoryAuthRepository();
				inMemoryRepo.Clear();
				yield return new TestCaseData(inMemoryRepo);

                var appSettings = new AppSettings();
                var redisRepo = new RedisAuthRepository(new BasicRedisClientManager(new string[] { appSettings.GetString("Redis.Host") ?? "localhost" }));
				redisRepo.Clear();
				yield return new TestCaseData(redisRepo);

				if (UseSqlServer)
				{
					var connStr = @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\App_Data\auth.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
					var sqlServerFactory = new OrmLiteConnectionFactory(connStr, SqlServerOrmLiteDialectProvider.Instance);
					var sqlServerRepo = new OrmLiteAuthRepository(sqlServerFactory);
					sqlServerRepo.DropAndReCreateTables();
					yield return new TestCaseData(sqlServerRepo);
				}
				else
				{
					var dbFactory = new OrmLiteConnectionFactory(
						":memory:", autoDisposeConnection:false, dialectProvider:SqliteDialect.Provider);
					var sqliteRepo = new OrmLiteAuthRepository(dbFactory);
					sqliteRepo.CreateMissingTables();
					sqliteRepo.Clear();
					yield return new TestCaseData(sqliteRepo);

					var dbFilePath = "~/App_Data/auth.sqlite".MapProjectPath();
					if (File.Exists(dbFilePath)) File.Delete(dbFilePath);
					var sqliteDbFactory = new OrmLiteConnectionFactory(dbFilePath);
					var sqliteDbRepo = new OrmLiteAuthRepository(sqliteDbFactory);
					sqliteDbRepo.CreateMissingTables();
					yield return new TestCaseData(sqliteDbRepo);
				}
			}
		}

        /// <summary>The mock service.</summary>
		protected Mock<IServiceBase> mockService;
        /// <summary>Context for the request.</summary>
		protected MockRequestContext requestContext;
        /// <summary>The service.</summary>
		protected IServiceBase service;

        /// <summary>The facebook gateway tokens.</summary>
		protected OAuthTokens facebookGatewayTokens = new OAuthTokens {
			UserId = "623501766",
			DisplayName = "Demis Bellot FB",
			FirstName = "Demis",
			LastName = "Bellot",
			Email = "demis.bellot@gmail.com",
		};
        /// <summary>The twitter gateway tokens.</summary>
		protected OAuthTokens twitterGatewayTokens = new OAuthTokens {
			DisplayName = "Demis Bellot TW"
		};
        /// <summary>The facebook authentication tokens.</summary>
		protected OAuthTokens facebookAuthTokens = new OAuthTokens {
			Provider = FacebookAuthProvider.Name,
			AccessTokenSecret = "AAADDDCCCoR848BAMkQIZCRIKnVWZAvcKWqo7Ibvec8ebV9vJrfZAz8qVupdu5EbjFzmMmbwUFDbcNDea9H6rOn5SVn8es7KYZD",
		};
        /// <summary>The twitter authentication tokens.</summary>
		protected OAuthTokens twitterAuthTokens = new OAuthTokens {
			Provider = TwitterAuthProvider.Name,
			RequestToken = "JGGZZ22CCqgB1GR5e0EmGFxzyxGTw2rwEFFcC8a9o7g",
			RequestTokenSecret = "qKKCCUUJ2R10bMieVQZZad7iSwWkPYJmtBYzPoM9q0",
			UserId = "133371690876022785",
		};
        /// <summary>The registration dto.</summary>
		protected Registration registrationDto;

        /// <summary>Initialises the test.</summary>
        ///
        /// <param name="userAuthRepository">The user authentication repository.</param>
		protected void InitTest(IUserAuthRepository userAuthRepository)
		{
			((IClearable)userAuthRepository).Clear();

			var appsettingsMock = new Mock<IResourceManager>();
			var appSettings = appsettingsMock.Object;

			new AuthFeature(null, new IAuthProvider[] {
				new CredentialsAuthProvider(),
				new BasicAuthProvider(),
				new FacebookAuthProvider(appSettings),
				new TwitterAuthProvider(appSettings)
			}).Register(null);

			mockService = new Mock<IServiceBase>();
			mockService.Expect(x => x.TryResolve<IUserAuthRepository>()).Returns(userAuthRepository);
			requestContext = new MockRequestContext();
			mockService.Expect(x => x.RequestContext).Returns(requestContext);
			service = mockService.Object;

			registrationDto = new Registration {
				UserName = "UserName",
				Password = "p@55word",
				Email = "as@if.com",
				DisplayName = "DisplayName",
				FirstName = "FirstName",
				LastName = "LastName",
			};
		}

        /// <summary>Gets registration service.</summary>
        ///
        /// <param name="userAuthRepository">The user authentication repository.</param>
        /// <param name="oAuthUserSession">  The authentication user session.</param>
        /// <param name="requestContext">    Context for the request.</param>
        ///
        /// <returns>The registration service.</returns>
		public static RegistrationService GetRegistrationService(
			IUserAuthRepository userAuthRepository,
			AuthUserSession oAuthUserSession = null,
			MockRequestContext requestContext = null)
		{
			if (requestContext == null)
				requestContext = new MockRequestContext();
			if (oAuthUserSession == null)
				oAuthUserSession = requestContext.ReloadSession();

			var httpReq = requestContext.Get<IHttpRequest>();
			var httpRes = requestContext.Get<IHttpResponse>();
			oAuthUserSession.Id = httpRes.CreateSessionId(httpReq);
			httpReq.Items[ServiceExtensions.RequestItemsSessionKey] = oAuthUserSession;

			var mockAppHost = new BasicAppHost {
				Container = requestContext.Container
			};

			requestContext.Container.Register(userAuthRepository);

		    var authService = new AuthService {
                RequestContext = requestContext,
            };
            authService.SetResolver(mockAppHost);
            mockAppHost.Register(authService);

			var registrationService = new RegistrationService {
				UserAuthRepo = userAuthRepository,
				RequestContext = requestContext,
				RegistrationValidator =
					new RegistrationValidator { UserAuthRepo = RegistrationServiceTests.GetStubRepo() },
			};
			registrationService.SetResolver(mockAppHost);

			return registrationService;
		}

        /// <summary>Assert equal.</summary>
        ///
        /// <param name="userAuth">The user authentication.</param>
        /// <param name="request"> The request.</param>
		public static void AssertEqual(UserAuth userAuth, Registration request)
		{
			Assert.That(userAuth, Is.Not.Null);
			Assert.That(userAuth.UserName, Is.EqualTo(request.UserName));
			Assert.That(userAuth.Email, Is.EqualTo(request.Email));
			Assert.That(userAuth.DisplayName, Is.EqualTo(request.DisplayName));
			Assert.That(userAuth.FirstName, Is.EqualTo(request.FirstName));
			Assert.That(userAuth.LastName, Is.EqualTo(request.LastName));
		}

        /// <summary>Registers the and login.</summary>
        ///
        /// <param name="userAuthRepository">The user authentication repository.</param>
        /// <param name="oAuthUserSession">  The authentication user session.</param>
        ///
        /// <returns>An AuthUserSession.</returns>
		protected AuthUserSession RegisterAndLogin(IUserAuthRepository userAuthRepository, AuthUserSession oAuthUserSession)
		{
			Register(userAuthRepository, oAuthUserSession);

			Login(registrationDto.UserName, registrationDto.Password, oAuthUserSession);

			oAuthUserSession = requestContext.ReloadSession();
			return oAuthUserSession;
		}

        /// <summary>Login.</summary>
        ///
        /// <param name="userName">        Name of the user.</param>
        /// <param name="password">        The password.</param>
        /// <param name="oAuthUserSession">The authentication user session.</param>
        ///
        /// <returns>An object.</returns>
		protected object Login(string userName, string password, AuthUserSession oAuthUserSession = null)
		{
			if (oAuthUserSession == null)
				oAuthUserSession = requestContext.ReloadSession();

			var credentialsAuth = GetCredentialsAuthConfig();
			return credentialsAuth.Authenticate(service, oAuthUserSession,
				new Auth {
					provider = CredentialsAuthProvider.Name,
					UserName = registrationDto.UserName,
					Password = registrationDto.Password,
				});
		}

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="userAuthRepository">The user authentication repository.</param>
        /// <param name="oAuthUserSession">  The authentication user session.</param>
        /// <param name="registration">      The registration.</param>
        ///
        /// <returns>An object.</returns>
		protected object Register(IUserAuthRepository userAuthRepository, AuthUserSession oAuthUserSession, Registration registration = null)
		{
			if (registration == null)
				registration = registrationDto;

			var registrationService = GetRegistrationService(userAuthRepository, oAuthUserSession, requestContext);
			var response = registrationService.Post(registration);
			Assert.That(response as IHttpError, Is.Null);
			return response;
		}

        /// <summary>Login with facebook.</summary>
        ///
        /// <param name="oAuthUserSession">The authentication user session.</param>
		protected void LoginWithFacebook(AuthUserSession oAuthUserSession)
		{
			MockAuthHttpGateway.Tokens = facebookGatewayTokens;
			var facebookAuth = GetFacebookAuthProvider();
			facebookAuth.OnAuthenticated(service, oAuthUserSession, facebookAuthTokens, new Dictionary<string, string>());
			Console.WriteLine("UserId: " + oAuthUserSession.UserAuthId);
		}
	}
}