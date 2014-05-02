using System.Linq;
using NUnit.Framework;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.ServiceInterface.Testing;

namespace NServiceKit.Common.Tests.OAuth
{
    /// <summary>A required roles tests.</summary>
    [TestFixture]
    public class RequiredRolesTests
    {
        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            AuthService.Init(() => new AuthUserSession(), new CredentialsAuthProvider());
        }

        /// <summary>A mock user authentication repository.</summary>
        public class MockUserAuthRepository : InMemoryAuthRepository
        {
            private UserAuth userAuth;

            /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.OAuth.RequiredRolesTests.MockUserAuthRepository class.</summary>
            ///
            /// <param name="userAuth">The user authentication.</param>
            public MockUserAuthRepository(UserAuth userAuth)
            {
                this.userAuth = userAuth;
            }

            /// <summary>Gets user authentication by user name.</summary>
            ///
            /// <param name="userNameOrEmail">The user name or email.</param>
            ///
            /// <returns>The user authentication by user name.</returns>
            public override UserAuth GetUserAuthByUserName(string userNameOrEmail)
            {
                return null;
            }

            /// <summary>Creates user authentication.</summary>
            ///
            /// <param name="newUser"> The new user.</param>
            /// <param name="password">The password.</param>
            ///
            /// <returns>The new user authentication.</returns>
            public override UserAuth CreateUserAuth(UserAuth newUser, string password)
            {
                return userAuth;
            }

            /// <summary>Gets user authentication.</summary>
            ///
            /// <param name="authSession">The authentication session.</param>
            /// <param name="tokens">     The tokens.</param>
            ///
            /// <returns>The user authentication.</returns>
            public override UserAuth GetUserAuth(IAuthSession authSession, IOAuthTokens tokens)
            {
                return userAuth;
            }

            /// <summary>Attempts to authenticate from the given data.</summary>
            ///
            /// <param name="userName">Name of the user.</param>
            /// <param name="password">The password.</param>
            /// <param name="userAuth">The user authentication.</param>
            ///
            /// <returns>true if it succeeds, false if it fails.</returns>
            public override bool TryAuthenticate(string userName, string password, out UserAuth userAuth)
            {
                userAuth = this.userAuth;
                return true;
            }
        }

        private MockUserAuthRepository userAuth;

        /// <summary>Sets the up.</summary>
        [SetUp]
        public void SetUp()
        {
            var userWithAdminRole = new UserAuth { Id = 1, Roles = new[] { RoleNames.Admin }.ToList() };
            userAuth = new MockUserAuthRepository(userWithAdminRole);
        }

        private RegistrationService GetRegistrationService()
        {
            var registrationService = RegistrationServiceTests.GetRegistrationService(authRepo: userAuth);
            var request = RegistrationServiceTests.GetValidRegistration(autoLogin: true);

            registrationService.Post(request);
            return registrationService;
        }

        /// <summary>Does validate required roles with user authentication repo when role not in session.</summary>
        [Test]
        public void Does_validate_RequiredRoles_with_UserAuthRepo_When_Role_not_in_Session()
        {
            var registrationService = GetRegistrationService();

            var requiredRole = new RequiredRoleAttribute(RoleNames.Admin);

            var requestContext = (MockRequestContext)registrationService.RequestContext;
            requestContext.Container.Register(userAuth);
            var httpRes = requestContext.Get<IHttpResponse>();

            requiredRole.Execute(
                requestContext.Get<IHttpRequest>(),
                httpRes,
                null);

            Assert.That(!httpRes.IsClosed);
        }

        /// <summary>Does validate assert required roles with user authentication repo when role not in session.</summary>
        [Test]
        public void Does_validate_AssertRequiredRoles_with_UserAuthRepo_When_Role_not_in_Session()
        {
            var registrationService = GetRegistrationService();

            var requestContext = (MockRequestContext)registrationService.RequestContext;
            requestContext.Container.Register(userAuth);
            var httpRes = requestContext.Get<IHttpResponse>();

            RequiredRoleAttribute.AssertRequiredRoles(requestContext, RoleNames.Admin);

            Assert.That(!httpRes.IsClosed);
        }
    }
}