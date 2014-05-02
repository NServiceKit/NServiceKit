using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using Funq;
using NUnit.Framework;
using NServiceKit.CacheAccess;
using NServiceKit.CacheAccess.Providers;
using NServiceKit.Common;
using NServiceKit.Common.Tests.ServiceClient.Web;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests.Support.Services;
using NServiceKit.WebHost.IntegrationTests.Services;
using System.Collections.Generic;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A secured.</summary>
    [Route("/secured")]
    public class Secured : IReturn<SecuredResponse>
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>A secured response.</summary>
    public class SecuredResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>A secured file upload.</summary>
    [Route("/securedfileupload")]
    public class SecuredFileUpload
    {
        /// <summary>Gets or sets the identifier of the customer.</summary>
        ///
        /// <value>The identifier of the customer.</value>
        public int CustomerId { get; set; }

        /// <summary>Gets or sets the name of the customer.</summary>
        ///
        /// <value>The name of the customer.</value>
        public string CustomerName { get; set; }
    }

    /// <summary>A secured service.</summary>
    [Authenticate]
    public class SecuredService : ServiceInterface.Service
    {
        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(Secured request)
        {
            return new SecuredResponse { Result = request.Name };
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(Secured request)
        {
            throw new ArgumentException("unicorn nuggets");
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(SecuredFileUpload request)
        {
            var file = this.RequestContext.Files[0];
            return new FileUploadResponse
            {
                FileName = file.FileName,
                ContentLength = file.ContentLength,
                ContentType = file.ContentType,
                Contents = new StreamReader(file.InputStream).ReadToEnd(),
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName
            };
        }
    }

    /// <summary>The requires role.</summary>
    public class RequiresRole
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>The requires role response.</summary>
    public class RequiresRoleResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>The requires role service.</summary>
    [RequiredRole("TheRole")]
    public class RequiresRoleService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(RequiresRole request)
        {
            return new RequiresRoleResponse { Result = request.Name };
        }
    }

    /// <summary>The requires any role.</summary>
    public class RequiresAnyRole
    {
        /// <summary>Gets or sets the roles.</summary>
        ///
        /// <value>The roles.</value>
        public List<string> Roles { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.RequiresAnyRole class.</summary>
        public RequiresAnyRole()
        {
            Roles = new List<string>();
        }
    }

    /// <summary>The requires any role response.</summary>
    public class RequiresAnyRoleResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public List<string> Result { get; set; }

        /// <summary>Gets or sets the repsonse status.</summary>
        ///
        /// <value>The repsonse status.</value>
        public ResponseStatus RepsonseStatus { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.RequiresAnyRoleResponse class.</summary>
        public RequiresAnyRoleResponse()
        {
            Result = new List<string>();
        }
    }

    /// <summary>The requires any role service.</summary>
    [RequiresAnyRole("TheRole", "TheRole2")]
    public class RequiresAnyRoleService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(RequiresAnyRole request)
        {
            return new RequiresAnyRoleResponse { Result = request.Roles };
        }
    }

    /// <summary>requires permission descriptor.</summary>
    public class RequiresPermission
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>The requires permission response.</summary>
    public class RequiresPermissionResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>The requires permission service.</summary>
    [RequiredPermission("ThePermission")]
    public class RequiresPermissionService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A RequiresPermissionResponse.</returns>
        public RequiresPermissionResponse Any(RequiresPermission request)
        {
            return new RequiresPermissionResponse { Result = request.Name };
        }
    }

    /// <summary>requires any permission descriptor.</summary>
    public class RequiresAnyPermission
    {
        /// <summary>Gets or sets the permissions.</summary>
        ///
        /// <value>The permissions.</value>
        public List<string> Permissions { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.RequiresAnyPermission class.</summary>
        public RequiresAnyPermission()
        {
            Permissions = new List<string>();
        }
    }

    /// <summary>The requires any permission response.</summary>
    public class RequiresAnyPermissionResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public List<string> Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.RequiresAnyPermissionResponse class.</summary>
        public RequiresAnyPermissionResponse()
        {
            Result = new List<string>();
        }
    }

    /// <summary>The requires any permission service.</summary>
    [RequiresAnyPermission("ThePermission", "ThePermission2")]
    public class RequiresAnyPermissionService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A RequiresAnyPermissionResponse.</returns>
        public RequiresAnyPermissionResponse Any(RequiresAnyPermission request)
        {
            return new RequiresAnyPermissionResponse { Result = request.Permissions };
        }
    }

    /// <summary>A custom user session.</summary>
    public class CustomUserSession : AuthUserSession
    {
        /// <summary>Executes the authenticated action.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        /// <param name="tokens">     The tokens.</param>
        /// <param name="authInfo">   Information describing the authentication.</param>
        public override void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, System.Collections.Generic.Dictionary<string, string> authInfo)
        {
            if (session.UserName == AuthTests.UserNameWithSessionRedirect)
                session.ReferrerUrl = AuthTests.SessionRedirectUrl;
        }
    }

    /// <summary>A custom authentication provider.</summary>
    public class CustomAuthProvider : AuthProvider
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.CustomAuthProvider class.</summary>
        public CustomAuthProvider()
        {
            this.Provider = "custom";
        }

        /// <summary>Query if 'session' is authorized.</summary>
        ///
        /// <param name="session">The session.</param>
        /// <param name="tokens"> The tokens.</param>
        /// <param name="request">The request.</param>
        ///
        /// <returns>true if authorized, false if not.</returns>
        public override bool IsAuthorized(IAuthSession session, IOAuthTokens tokens, Auth request = null)
        {
            return false;
        }

        /// <summary>Authenticates.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        /// <param name="request">    The request.</param>
        ///
        /// <returns>An object.</returns>
        public override object Authenticate(IServiceBase authService, IAuthSession session, Auth request)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>The requires custom authentication.</summary>
    public class RequiresCustomAuth
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>The requires custom authentication response.</summary>
    public class RequiresCustomAuthResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>The requires custom authentication service.</summary>
    [Authenticate(Provider = "custom")]
    public class RequiresCustomAuthService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A RequiresCustomAuthResponse.</returns>
        public RequiresCustomAuthResponse Any(RequiresCustomAuth request)
        {
            return new RequiresCustomAuthResponse { Result = request.Name };
        }
    }

    /// <summary>Attribute for custom authenticate.</summary>
    public class CustomAuthenticateAttribute : NServiceKit.ServiceInterface.AuthenticateAttribute
    {
        /// <summary>Executes.</summary>
        ///
        /// <param name="req">       The request.</param>
        /// <param name="res">       The resource.</param>
        /// <param name="requestDto">The request dto.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            //Need to run SessionFeature filter since its not executed before this attribute (Priority -100)
            SessionFeature.AddSessionIdToRequestFilter(req, res, null); //Required to get req.GetSessionId()

            req.Items["TriedMyOwnAuthFirst"] = true; // let's simulate some sort of auth _before_ relaying to base class.

            base.Execute(req, res, requestDto);
        }
    }

    /// <summary>Attribute for custom authentication.</summary>
    public class CustomAuthAttr
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>A custom authentication attribute response.</summary>
    public class CustomAuthAttrResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>A custom authentication attribute service.</summary>
    [CustomAuthenticate]
    public class CustomAuthAttrService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A CustomAuthAttrResponse.</returns>
        public CustomAuthAttrResponse Any(CustomAuthAttr request)
        {
            if (!Request.Items.ContainsKey("TriedMyOwnAuthFirst"))
                throw new InvalidOperationException("TriedMyOwnAuthFirst not present.");

            return new CustomAuthAttrResponse { Result = request.Name };
        }
    }

    /// <summary>An authentication tests.</summary>
    public class AuthTests
    {
        /// <summary>Gets the pathname of the virtual directory.</summary>
        ///
        /// <value>The pathname of the virtual directory.</value>
        protected virtual string VirtualDirectory { get { return ""; } }

        /// <summary>Gets or sets the listening on.</summary>
        ///
        /// <value>The listening on.</value>
        protected virtual string ListeningOn { get { return "http://localhost:82/"; } }

        /// <summary>Gets or sets URL of the web host.</summary>
        ///
        /// <value>The web host URL.</value>
        protected virtual string WebHostUrl { get { return "http://mydomain.com"; } }

        private const string UserName = "user";
        private const string Password = "p@55word";
        /// <summary>The user name with session redirect.</summary>
        public const string UserNameWithSessionRedirect = "user2";
        /// <summary>The password for session redirect.</summary>
        public const string PasswordForSessionRedirect = "p@55word2";
        /// <summary>URL of the session redirect.</summary>
        public const string SessionRedirectUrl = "specialLandingPage.html";
        /// <summary>URL of the login.</summary>
        public const string LoginUrl = "specialLoginPage.html";
        private const string EmailBasedUsername = "user@email.com";
        private const string PasswordForEmailBasedAccount = "p@55word3";


        /// <summary>An authentication application host HTTP listener.</summary>
        public class AuthAppHostHttpListener
            : AppHostHttpListenerBase
        {
            private readonly string webHostUrl;

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.AuthTests.AuthAppHostHttpListener class.</summary>
            ///
            /// <param name="webHostUrl">URL of the web host.</param>
            public AuthAppHostHttpListener(string webHostUrl)
                : base("Validation Tests", typeof (CustomerService).Assembly)
            {
                this.webHostUrl = webHostUrl;
            }

            private InMemoryAuthRepository userRep;

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Container container)
            {
                SetConfig(new EndpointHostConfig { WebHostUrl = webHostUrl });

                Plugins.Add(new AuthFeature(() => new CustomUserSession(),
                    new IAuthProvider[] { //Www-Authenticate should contain basic auth, therefore register this provider first
                        new BasicAuthProvider(), //Sign-in with Basic Auth
						new CredentialsAuthProvider(), //HTML Form post of UserName/Password credentials
                        new CustomAuthProvider()
					}, "~/" + LoginUrl));

                container.Register<ICacheClient>(new MemoryCacheClient());
                userRep = new InMemoryAuthRepository();
                container.Register<IUserAuthRepository>(userRep);

                CreateUser(1, UserName, null, Password, new List<string> { "TheRole" }, new List<string> { "ThePermission" });
                CreateUser(2, UserNameWithSessionRedirect, null, PasswordForSessionRedirect);
                CreateUser(3, null, EmailBasedUsername, PasswordForEmailBasedAccount);
            }

            private void CreateUser(int id, string username, string email, string password, List<string> roles = null, List<string> permissions = null)
            {
                string hash;
                string salt;
                new SaltedHash().GetHashAndSaltString(password, out hash, out salt);

                userRep.CreateUserAuth(new UserAuth
                {
                    Id = id,
                    DisplayName = "DisplayName",
                    Email = email ?? "as@if{0}.com".Fmt(id),
                    UserName = username,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    PasswordHash = hash,
                    Salt = salt,
                    Roles = roles,
                    Permissions = permissions
                }, password);
            }

            /// <summary>Releases the unmanaged resources used by the NServiceKit.WebHost.Endpoints.Tests.AuthTests.AuthAppHostHttpListener and optionally releases the managed resources.</summary>
            ///
            /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
            protected override void Dispose(bool disposing)
            {
                // Needed so that when the derived class tests run the same users can be added again.
                userRep.Clear();
                base.Dispose(disposing);
            }
        }

        AuthAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            appHost = new AuthAppHostHttpListener(WebHostUrl);
            appHost.Init();
            appHost.Start(ListeningOn);
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            appHost.Dispose();
        }

        private static void FailOnAsyncError<T>(T response, Exception ex)
        {
            Assert.Fail(ex.Message);
        }

        IServiceClient GetClient()
        {
            return new JsonServiceClient(ListeningOn);
        }

        IServiceClient GetHtmlClient()
        {
            return new HtmlServiceClient(ListeningOn) {BaseUri = ListeningOn};
        }

        IServiceClient GetClientWithUserPassword()
        {
            return new JsonServiceClient(ListeningOn)
            {
                UserName = UserName,
                Password = Password
            };
        }

        /// <summary>No credentials throws un authorized.</summary>
        [Test]
        public void No_Credentials_throws_UnAuthorized()
        {
            try
            {
                var client = GetClient();
                var request = new Secured { Name = "test" };
                var response = client.Send<SecureResponse>(request);

                Assert.Fail("Shouldn't be allowed");
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Authenticate attribute respects provider.</summary>
        [Test]
        public void Authenticate_attribute_respects_provider()
        {
            try
            {
                var client = GetClient();
                var authResponse = client.Send(new Auth
                {
                    provider = CredentialsAuthProvider.Name,
                    UserName = "user",
                    Password = "p@55word",
                    RememberMe = true,
                });

                var request = new RequiresCustomAuth { Name = "test" };
                var response = client.Send<RequiresCustomAuthResponse>(request);

                Assert.Fail("Shouldn't be allowed");
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Posts the file with no credentials throws un authorized.</summary>
        [Test]
        public void PostFile_with_no_Credentials_throws_UnAuthorized()
        {
            try
            {
                var client = GetClient();
                var uploadFile = new FileInfo("~/TestExistingDir/upload.html".MapProjectPath());
                client.PostFile<FileUploadResponse>(ListeningOn + "/securedfileupload", uploadFile, MimeTypes.GetMimeType(uploadFile.Name));

                Assert.Fail("Shouldn't be allowed");
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Posts the file does work with basic authentication.</summary>
        [Test]
        public void PostFile_does_work_with_BasicAuth()
        {
            var client = GetClientWithUserPassword();
            var uploadFile = new FileInfo("~/TestExistingDir/upload.html".MapProjectPath());

            var expectedContents = new StreamReader(uploadFile.OpenRead()).ReadToEnd();
            var response = client.PostFile<FileUploadResponse>(ListeningOn + "/securedfileupload", uploadFile, MimeTypes.GetMimeType(uploadFile.Name));
            Assert.That(response.FileName, Is.EqualTo(uploadFile.Name));
            Assert.That(response.ContentLength, Is.EqualTo(uploadFile.Length));
            Assert.That(response.Contents, Is.EqualTo(expectedContents));
        }

        /// <summary>Posts the file with request does work with basic authentication.</summary>
        [Test]
        public void PostFileWithRequest_does_work_with_BasicAuth()
        {
            var client = GetClientWithUserPassword();
            var request = new SecuredFileUpload { CustomerId = 123, CustomerName = "Foo" };
            var uploadFile = new FileInfo("~/TestExistingDir/upload.html".MapProjectPath());

            var expectedContents = new StreamReader(uploadFile.OpenRead()).ReadToEnd();
            var response = client.PostFileWithRequest<FileUploadResponse>(ListeningOn + "/securedfileupload", uploadFile, request);
            Assert.That(response.FileName, Is.EqualTo(uploadFile.Name));
            Assert.That(response.ContentLength, Is.EqualTo(uploadFile.Length));
            Assert.That(response.Contents, Is.EqualTo(expectedContents));
            Assert.That(response.CustomerName, Is.EqualTo("Foo"));
            Assert.That(response.CustomerId, Is.EqualTo(123));
        }

        /// <summary>Does work with basic authentication.</summary>
        [Test]
        public void Does_work_with_BasicAuth()
        {
            try
            {
                var client = GetClientWithUserPassword();
                var request = new Secured { Name = "test" };
                var response = client.Send<SecureResponse>(request);
                Assert.That(response.Result, Is.EqualTo(request.Name));
            }
            catch (WebServiceException webEx)
            {
                Assert.Fail(webEx.Message);
            }
        }

        /// <summary>Does always send basic authentication.</summary>
        [Test]
        public void Does_always_send_BasicAuth()
        {
            try
            {
                var client = (ServiceClientBase)GetClientWithUserPassword();
                client.AlwaysSendBasicAuthHeader = true;
                client.LocalHttpWebRequestFilter = req =>
                {
                    bool hasAuthentication = false;
                    foreach (var key in req.Headers.Keys)
                    {
                        if (key.ToString() == "Authorization")
                            hasAuthentication = true;
                    }
                    Assert.IsTrue(hasAuthentication);
                };

                var request = new Secured { Name = "test" };
                var response = client.Send<SecureResponse>(request);
                Assert.That(response.Result, Is.EqualTo(request.Name));
            }
            catch (WebServiceException webEx)
            {
                Assert.Fail(webEx.Message);
            }
        }

        /// <summary>Does work with credentails authentication.</summary>
        [Test]
        public void Does_work_with_CredentailsAuth()
        {
            try
            {
                var client = GetClient();

                var authResponse = client.Send(new Auth
                {
                    provider = CredentialsAuthProvider.Name,
                    UserName = "user",
                    Password = "p@55word",
                    RememberMe = true,
                });

                authResponse.PrintDump();

                var request = new Secured { Name = "test" };
                var response = client.Send<SecureResponse>(request);
                Assert.That(response.Result, Is.EqualTo(request.Name));
            }
            catch (WebServiceException webEx)
            {
                Assert.Fail(webEx.Message);
            }
        }

        /// <summary>Does work with credentails authentication asynchronous.</summary>
        [Test]
        public void Does_work_with_CredentailsAuth_Async()
        {
            var client = GetClient();

            var request = new Secured { Name = "test" };
            SecureResponse response = null;

            client.SendAsync<AuthResponse>(new Auth
            {
                provider = CredentialsAuthProvider.Name,
                UserName = "user",
                Password = "p@55word",
                RememberMe = true,
            }, authResponse =>
            {
                authResponse.PrintDump();
                client.SendAsync<SecureResponse>(request, r => response = r, FailOnAsyncError);

            }, FailOnAsyncError);

            Thread.Sleep(TimeSpan.FromSeconds(1));
            Assert.That(response.Result, Is.EqualTo(request.Name));
        }

        /// <summary>Can call required role service with basic authentication.</summary>
        [Test]
        public void Can_call_RequiredRole_service_with_BasicAuth()
        {
            try
            {
                var client = GetClientWithUserPassword();
                var request = new RequiresRole { Name = "test" };
                var response = client.Send<RequiresRoleResponse>(request);
                Assert.That(response.Result, Is.EqualTo(request.Name));
            }
            catch (WebServiceException webEx)
            {
                Assert.Fail(webEx.Message);
            }
        }

        /// <summary>Queries if a given required role service returns unauthorized if no basic authentication header exists.</summary>
        [Test]
        public void RequiredRole_service_returns_unauthorized_if_no_basic_auth_header_exists()
        {
            try
            {
                var client = GetClient();
                var request = new RequiresRole { Name = "test" };
                var response = client.Send<RequiresRoleResponse>(request);
                Assert.Fail();
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Queries if a given required role service returns forbidden if basic authentication header exists.</summary>
        [Test]
        public void RequiredRole_service_returns_forbidden_if_basic_auth_header_exists()
        {
            try
            {
                var client = GetClient();
                ((ServiceClientBase)client).UserName = EmailBasedUsername;
                ((ServiceClientBase)client).Password = PasswordForEmailBasedAccount;

                var request = new RequiresRole { Name = "test" };
                var response = client.Send<RequiresRoleResponse>(request);
                Assert.Fail();
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Can call required permission service with basic authentication.</summary>
        [Test]
        public void Can_call_RequiredPermission_service_with_BasicAuth()
        {
            try
            {
                var client = GetClientWithUserPassword();
                var request = new RequiresPermission { Name = "test" };
                var response = client.Send<RequiresPermissionResponse>(request);
                Assert.That(response.Result, Is.EqualTo(request.Name));
            }
            catch (WebServiceException webEx)
            {
                Assert.Fail(webEx.Message);
            }
        }

        /// <summary>Queries if a given required permission service returns unauthorized if no basic authentication header exists.</summary>
        [Test]
        public void RequiredPermission_service_returns_unauthorized_if_no_basic_auth_header_exists()
        {
            try
            {
                var client = GetClient();
                var request = new RequiresPermission { Name = "test" };
                var response = client.Send<RequiresPermissionResponse>(request);
                Assert.Fail();
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Queries if a given required permission service returns forbidden if basic authentication header exists.</summary>
        [Test]
        public void RequiredPermission_service_returns_forbidden_if_basic_auth_header_exists()
        {
            try
            {
                var client = GetClient();
                ((ServiceClientBase)client).UserName = EmailBasedUsername;
                ((ServiceClientBase)client).Password = PasswordForEmailBasedAccount;

                var request = new RequiresPermission { Name = "test" };
                var response = client.Send<RequiresPermissionResponse>(request);
                Assert.Fail();
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Does work with credentails authentication multiple times.</summary>
        [Test]
        public void Does_work_with_CredentailsAuth_Multiple_Times()
        {
            try
            {
                var client = GetClient();

                var authResponse = client.Send<AuthResponse>(new Auth
                {
                    provider = CredentialsAuthProvider.Name,
                    UserName = "user",
                    Password = "p@55word",
                    RememberMe = true,
                });

                Console.WriteLine(authResponse.Dump());

                for (int i = 0; i < 500; i++)
                {
                    var request = new Secured { Name = "test" };
                    var response = client.Send<SecureResponse>(request);
                    Assert.That(response.Result, Is.EqualTo(request.Name));
                    Console.WriteLine("loop : {0}", i);
                }
            }
            catch (WebServiceException webEx)
            {
                Assert.Fail(webEx.Message);
            }
        }

        /// <summary>Exceptions thrown are received by client when always send basic authentication header is false.</summary>
        [Test]
        public void Exceptions_thrown_are_received_by_client_when_AlwaysSendBasicAuthHeader_is_false()
        {
            try
            {
                var client = (IRestClient)GetClientWithUserPassword();
                ((ServiceClientBase)client).AlwaysSendBasicAuthHeader = false;
                var response = client.Get<SecuredResponse>("/secured");

                Assert.Fail("Should have thrown");
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.ErrorMessage, Is.EqualTo("unicorn nuggets"));
            }
        }

        /// <summary>Exceptions thrown are received by client when always send basic authentication header is true.</summary>
        [Test]
        public void Exceptions_thrown_are_received_by_client_when_AlwaysSendBasicAuthHeader_is_true()
        {
            try
            {
                var client = (IRestClient)GetClientWithUserPassword();
                ((ServiceClientBase)client).AlwaysSendBasicAuthHeader = true;
                var response = client.Get<SecuredResponse>("/secured");

                Assert.Fail("Should have thrown");
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.ErrorMessage, Is.EqualTo("unicorn nuggets"));
            }
        }

        /// <summary>HTML clients receive redirect to login page when accessing unauthenticated.</summary>
        [Test]
        public void Html_clients_receive_redirect_to_login_page_when_accessing_unauthenticated()
        {
            var client = (ServiceClientBase)GetHtmlClient();
            client.AllowAutoRedirect = false;
            string lastResponseLocationHeader = null;
            client.LocalHttpWebResponseFilter = response =>
            {
                lastResponseLocationHeader = response.Headers["Location"];
            };

            var request = new Secured { Name = "test" };
            client.Send<SecureResponse>(request);

            var locationUri = new Uri(lastResponseLocationHeader);
            var loginPath = "/".CombineWith(VirtualDirectory).CombineWith(LoginUrl);
            Assert.That(locationUri.AbsolutePath, Is.EqualTo(loginPath).IgnoreCase);
        }

        /// <summary>HTML clients receive secured URL attempt in login page redirect query string.</summary>
        [Test]
        public void Html_clients_receive_secured_url_attempt_in_login_page_redirect_query_string()
        {
            var client = (ServiceClientBase)GetHtmlClient();
            client.AllowAutoRedirect = false;
            string lastResponseLocationHeader = null;
            client.LocalHttpWebResponseFilter = response =>
            {
                lastResponseLocationHeader = response.Headers["Location"];
            };

            var request = new Secured { Name = "test" };
            client.Send<SecureResponse>(request);

            var locationUri = new Uri(lastResponseLocationHeader);
            var queryString = HttpUtility.ParseQueryString(locationUri.Query);
            var redirectQueryString = queryString["redirect"];
            var redirectUri = new Uri(redirectQueryString);

            // Should contain the url attempted to access before the redirect to the login page.
            var securedPath = "/".CombineWith(VirtualDirectory).CombineWith("secured");
            Assert.That(redirectUri.AbsolutePath, Is.EqualTo(securedPath).IgnoreCase);
            // The url should also obey the WebHostUrl setting for the domain.
            var redirectSchemeAndHost = redirectUri.Scheme + "://" + redirectUri.Authority;
            var webHostUri = new Uri(WebHostUrl);
            var webHostSchemeAndHost = webHostUri.Scheme + "://" + webHostUri.Authority;
            Assert.That(redirectSchemeAndHost, Is.EqualTo(webHostSchemeAndHost).IgnoreCase);
        }

        /// <summary>HTML clients receive secured URL including query string within login page redirect query string.</summary>
        [Test]
        public void Html_clients_receive_secured_url_including_query_string_within_login_page_redirect_query_string()
        {
            var client = (ServiceClientBase)GetHtmlClient();
            client.AllowAutoRedirect = false;
            string lastResponseLocationHeader = null;
            client.LocalHttpWebResponseFilter = response =>
            {
                lastResponseLocationHeader = response.Headers["Location"];
            };

            var request = new Secured { Name = "test" };
            // Perform a GET so that the Name DTO field is encoded as query string.
            client.Get(request);

            var locationUri = new Uri(lastResponseLocationHeader);
            var locationUriQueryString = HttpUtility.ParseQueryString(locationUri.Query);
            var redirectQueryItem = locationUriQueryString["redirect"];
            var redirectUri = new Uri(redirectQueryItem);

            // Should contain the url attempted to access before the redirect to the login page,
            // including the 'Name=test' query string.
            var redirectUriQueryString = HttpUtility.ParseQueryString(redirectUri.Query);
            Assert.That(redirectUriQueryString.AllKeys, Contains.Item("name"));
            Assert.That(redirectUriQueryString["name"], Is.EqualTo("test"));
        }

        /// <summary>HTML clients receive session referrer URL on successful authentication.</summary>
        [Test]
        public void Html_clients_receive_session_ReferrerUrl_on_successful_authentication()
        {
            var client = (ServiceClientBase)GetHtmlClient();
            client.AllowAutoRedirect = false;
            string lastResponseLocationHeader = null;
            client.LocalHttpWebResponseFilter = response =>
            {
                lastResponseLocationHeader = response.Headers["Location"];
            };

            client.Send(new Auth
            {
                provider = CredentialsAuthProvider.Name,
                UserName = UserNameWithSessionRedirect,
                Password = PasswordForSessionRedirect,
                RememberMe = true,
            });

            Assert.That(lastResponseLocationHeader, Is.EqualTo(SessionRedirectUrl));
        }

        /// <summary>Already authenticated session returns correct username.</summary>
        public void Already_authenticated_session_returns_correct_username()
        {
            var client = GetClient();

            var authRequest = new Auth
            {
                provider = CredentialsAuthProvider.Name,
                UserName = UserName,
                Password = Password,
                RememberMe = true,
            };
            var initialLoginResponse = client.Send(authRequest);
            var alreadyLogggedInResponse = client.Send(authRequest);

            Assert.That(alreadyLogggedInResponse.UserName, Is.EqualTo(UserName));
        }


        /// <summary>Authentication response returns email as username if user registered with email.</summary>
        [Test]
        public void AuthResponse_returns_email_as_username_if_user_registered_with_email()
        {
            var client = GetClient();

            var authRequest = new Auth
            {
                provider = CredentialsAuthProvider.Name,
                UserName = EmailBasedUsername,
                Password = PasswordForEmailBasedAccount,
                RememberMe = true,
            };
            var authResponse = client.Send(authRequest);

            Assert.That(authResponse.UserName, Is.EqualTo(EmailBasedUsername));
        }

        /// <summary>Already authenticated session returns correct username when user registered with email.</summary>
        [Test]
        public void Already_authenticated_session_returns_correct_username_when_user_registered_with_email()
        {
            var client = GetClient();

            var authRequest = new Auth
            {
                provider = CredentialsAuthProvider.Name,
                UserName = EmailBasedUsername,
                Password = PasswordForEmailBasedAccount,
                RememberMe = true,
            };
            var initialLoginResponse = client.Send(authRequest);
            var alreadyLogggedInResponse = client.Send(authRequest);

            Assert.That(initialLoginResponse.UserName, Is.EqualTo(EmailBasedUsername));
            Assert.That(alreadyLogggedInResponse.UserName, Is.EqualTo(EmailBasedUsername));
        }

        /// <summary>Can call requires any role service with basic authentication.</summary>
        [Test]
        public void Can_call_RequiresAnyRole_service_with_BasicAuth()
        {
            try
            {
                var client = GetClientWithUserPassword();
                var roles = new List<string>() {
                    "test", "test2"
                };
                var request = new RequiresAnyRole { Roles = roles };
                var response = client.Send<RequiresAnyRoleResponse>(request);
                Assert.That(response.Result, Is.EqualTo(request.Roles));
            }
            catch (WebServiceException webEx)
            {
                Assert.Fail(webEx.Message);
            }
        }

        /// <summary>Queries if a given requires any role service returns unauthorized if no basic authentication header exists.</summary>
        [Test]
        public void RequiresAnyRole_service_returns_unauthorized_if_no_basic_auth_header_exists()
        {
            try
            {
                var client = GetClient();
                var roles = new List<string>() {
                    "test", "test2"
                };
                var request = new RequiresAnyRole { Roles = roles };
                var response = client.Send<RequiresAnyRole>(request);
                Assert.Fail();
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Queries if a given requires any role service returns forbidden if basic authentication header exists.</summary>
        [Test]
        public void RequiresAnyRole_service_returns_forbidden_if_basic_auth_header_exists()
        {
            try
            {
                var client = GetClient();
                ((ServiceClientBase)client).UserName = EmailBasedUsername;
                ((ServiceClientBase)client).Password = PasswordForEmailBasedAccount;

                var roles = new List<string>() {
                    "test", "test2"
                };
                var request = new RequiresAnyRole { Roles = roles };
                var response = client.Send<RequiresAnyRoleResponse>(request);
                Assert.Fail();
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Can call requires any permission service with basic authentication.</summary>
        [Test]
        public void Can_call_RequiresAnyPermission_service_with_BasicAuth()
        {
            try
            {
                var client = GetClientWithUserPassword();
                var permissions = new List<string>
                {
                    "test", "test2"
                };
                var request = new RequiresAnyPermission { Permissions = permissions };
                var response = client.Send<RequiresAnyPermissionResponse>(request);
                Assert.That(response.Result, Is.EqualTo(request.Permissions));
            }
            catch (WebServiceException webEx)
            {
                Assert.Fail(webEx.Message);
            }
        }

        /// <summary>Queries if a given requires any permission service returns unauthorized if no basic authentication header exists.</summary>
        [Test]
        public void RequiresAnyPermission_service_returns_unauthorized_if_no_basic_auth_header_exists()
        {
            try
            {
                var client = GetClient();
                var permissions = new List<string>
                {
                    "test", "test2"
                };
                var request = new RequiresAnyPermission { Permissions = permissions };
                var response = client.Send<RequiresAnyPermissionResponse>(request);
                Assert.Fail();
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Queries if a given requires any permission service returns forbidden if basic authentication header exists.</summary>
        [Test]
        public void RequiresAnyPermission_service_returns_forbidden_if_basic_auth_header_exists()
        {
            try
            {
                var client = GetClient();
                ((ServiceClientBase)client).UserName = EmailBasedUsername;
                ((ServiceClientBase)client).Password = PasswordForEmailBasedAccount;
                var permissions = new List<string>
                {
                    "test", "test2"
                };
                var request = new RequiresAnyPermission { Permissions = permissions };
                var response = client.Send<RequiresAnyPermissionResponse>(request);
                Assert.Fail();
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
                Console.WriteLine(webEx.ResponseDto.Dump());
            }
        }

        /// <summary>Calling add session identifier to request from a custom authentication attribute does not duplicate session cookies.</summary>
        [Test]
        public void Calling_AddSessionIdToRequest_from_a_custom_auth_attribute_does_not_duplicate_session_cookies()
        {
            WebHeaderCollection headers = null;
            var client = GetClientWithUserPassword();
            ((ServiceClientBase)client).AlwaysSendBasicAuthHeader = true;
            ((ServiceClientBase)client).LocalHttpWebResponseFilter = x => headers = x.Headers;
            var response = client.Send<CustomAuthAttrResponse>(new CustomAuthAttr() { Name = "Hi You" });
            Assert.That(response.Result, Is.EqualTo("Hi You"));
            Assert.That(
                System.Text.RegularExpressions.Regex.Matches(headers["Set-Cookie"], "ss-id=").Count,
                Is.EqualTo(1)
            );
        }

        /// <summary>Meaningful exception for unknown authentication header.</summary>
        [TestCase(ExpectedException = typeof(AuthenticationException))]
        public void Meaningful_Exception_for_Unknown_Auth_Header()
        {
	        AuthenticationInfo authInfo = new AuthenticationInfo("Negotiate,NTLM");
        }
    }

    /// <summary>An authentication tests within virtual directory.</summary>
    public class AuthTestsWithinVirtualDirectory : AuthTests
    {
        /// <summary>Gets the pathname of the virtual directory.</summary>
        ///
        /// <value>The pathname of the virtual directory.</value>
        protected override string VirtualDirectory { get { return "somevirtualdirectory"; } }

        /// <summary>Gets or sets the listening on.</summary>
        ///
        /// <value>The listening on.</value>
        protected override string ListeningOn { get { return "http://localhost:82/" + VirtualDirectory + "/"; } }

        /// <summary>Gets URL of the web host.</summary>
        ///
        /// <value>The web host URL.</value>
        protected override string WebHostUrl { get { return "http://mydomain.com/" + VirtualDirectory; } }
    }
}