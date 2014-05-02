using NServiceKit.Common.Web;
using NServiceKit.Configuration;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>A basic authentication provider.</summary>
    public class BasicAuthProvider : CredentialsAuthProvider
    {
        /// <summary>The name.</summary>
        public new static string Name = AuthService.BasicProvider;
        /// <summary>The realm.</summary>
        public new static string Realm = "/auth/" + AuthService.BasicProvider;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.BasicAuthProvider class.</summary>
        public BasicAuthProvider()
        {
            this.Provider = Name;
            this.AuthRealm = Realm;
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.BasicAuthProvider class.</summary>
        ///
        /// <param name="appSettings">The application settings.</param>
        public BasicAuthProvider(IResourceManager appSettings)
            : base(appSettings, Realm, Name)
        {
        }

        /// <summary>The entry point for all AuthProvider providers. Runs inside the AuthService so exceptions are treated normally. Overridable so you can provide your own Auth implementation.</summary>
        ///
        /// <exception cref="Unauthorized">Thrown when an unauthorized error condition occurs.</exception>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        /// <param name="request">    The request.</param>
        ///
        /// <returns>An object.</returns>
        public override object Authenticate(IServiceBase authService, IAuthSession session, Auth request)
        {
            var httpReq = authService.RequestContext.Get<IHttpRequest>();
            var basicAuth = httpReq.GetBasicAuthUserAndPassword();
            if (basicAuth == null)
                throw HttpError.Unauthorized("Invalid BasicAuth credentials");

            var userName = basicAuth.Value.Key;
            var password = basicAuth.Value.Value;

            return Authenticate(authService, session, userName, password, request.Continue);
        }

        
    }
}