using System;
using System.Configuration;
using System.Runtime.Serialization;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>
    /// Inject logic into existing services by introspecting the request and injecting your own
    /// validation logic. Exceptions thrown will have the same behaviour as if the service threw it.
    /// 
    /// If a non-null object is returned the request will short-circuit and return that response.
    /// </summary>
    /// <param name="service">The instance of the service</param>
    /// <param name="httpMethod">GET,POST,PUT,DELETE</param>
    /// <param name="requestDto"></param>
    /// <returns>Response DTO; non-null will short-circuit execution and return that response</returns>
    public delegate object ValidateFn(IServiceBase service, string httpMethod, object requestDto);

    /// <summary>An authentication.</summary>
    [DataContract]
    public class Auth : IReturn<AuthResponse>
    {
        /// <summary>Gets or sets the provider.</summary>
        ///
        /// <value>The provider.</value>
        [DataMember(Order=1)] public string provider { get; set; }

        /// <summary>Gets or sets the state.</summary>
        ///
        /// <value>The state.</value>
        [DataMember(Order=2)] public string State { get; set; }

        /// <summary>Gets or sets the oauth token.</summary>
        ///
        /// <value>The oauth token.</value>
        [DataMember(Order=3)] public string oauth_token { get; set; }

        /// <summary>Gets or sets the oauth verifier.</summary>
        ///
        /// <value>The oauth verifier.</value>
        [DataMember(Order=4)] public string oauth_verifier { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        [DataMember(Order=5)] public string UserName { get; set; }

        /// <summary>Gets or sets the password.</summary>
        ///
        /// <value>The password.</value>
        [DataMember(Order=6)] public string Password { get; set; }

        /// <summary>Gets or sets the remember me.</summary>
        ///
        /// <value>The remember me.</value>
        [DataMember(Order=7)] public bool? RememberMe { get; set; }

        /// <summary>Gets or sets the continue.</summary>
        ///
        /// <value>The continue.</value>
        [DataMember(Order=8)] public string Continue { get; set; }
        // Thise are used for digest auth
        [DataMember(Order=9)] public string nonce { get; set; }

        /// <summary>Gets or sets URI of the document.</summary>
        ///
        /// <value>The URI.</value>
        [DataMember(Order=10)] public string uri { get; set; }

        /// <summary>Gets or sets the response.</summary>
        ///
        /// <value>The response.</value>
        [DataMember(Order=11)] public string response { get; set; }

        /// <summary>Gets or sets the qop.</summary>
        ///
        /// <value>The qop.</value>
        [DataMember(Order=12)] public string qop { get; set; }

        /// <summary>Gets or sets the non-client.</summary>
        ///
        /// <value>The non-client.</value>
        [DataMember(Order=13)] public string nc { get; set; }

        /// <summary>Gets or sets the cnonce.</summary>
        ///
        /// <value>The cnonce.</value>
        [DataMember(Order=14)] public string cnonce { get; set; }
    }

    /// <summary>An authentication response.</summary>
    [DataContract]
    public class AuthResponse
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.AuthResponse class.</summary>
        public AuthResponse()
        {
            this.ResponseStatus = new ResponseStatus();
        }

        /// <summary>Gets or sets the identifier of the session.</summary>
        ///
        /// <value>The identifier of the session.</value>
        [DataMember(Order=1)] public string SessionId { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        [DataMember(Order=2)] public string UserName { get; set; }

        /// <summary>Gets or sets URL of the referrer.</summary>
        ///
        /// <value>The referrer URL.</value>
        [DataMember(Order=3)] public string ReferrerUrl { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        [DataMember(Order=4)] public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>An authentication service.</summary>
    [DefaultRequest(typeof(Auth))]
    public class AuthService : Service
    {
        /// <summary>The basic provider.</summary>
        public const string BasicProvider = "basic";
        /// <summary>The credentials provider.</summary>
        public const string CredentialsProvider = "credentials";
        /// <summary>The logout action.</summary>
        public const string LogoutAction = "logout";
        /// <summary>The digest provider.</summary>
        public const string DigestProvider = "digest";

        /// <summary>Gets or sets the current session factory.</summary>
        ///
        /// <value>The current session factory.</value>
        public static Func<IAuthSession> CurrentSessionFactory { get; set; }

        /// <summary>Gets or sets the validate function.</summary>
        ///
        /// <value>The validate function.</value>
        public static ValidateFn ValidateFn { get; set; }

        /// <summary>Gets the default o authentication provider.</summary>
        ///
        /// <value>The default o authentication provider.</value>
        public static string DefaultOAuthProvider { get; private set; }

        /// <summary>Gets the default o authentication realm.</summary>
        ///
        /// <value>The default o authentication realm.</value>
        public static string DefaultOAuthRealm { get; private set; }

        /// <summary>Gets the HTML redirect.</summary>
        ///
        /// <value>The HTML redirect.</value>
        public static string HtmlRedirect { get; internal set; }

        /// <summary>Gets the authentication providers.</summary>
        ///
        /// <value>The authentication providers.</value>
        public static IAuthProvider[] AuthProviders { get; private set; }


        static AuthService()
        {
            CurrentSessionFactory = () => new AuthUserSession();
        }

        /// <summary>Gets authentication provider.</summary>
        ///
        /// <param name="provider">The provider.</param>
        ///
        /// <returns>The authentication provider.</returns>
        public static IAuthProvider GetAuthProvider(string provider)
        {
            if (AuthProviders == null || AuthProviders.Length == 0) return null;
            if (provider == LogoutAction) return AuthProviders[0];

            foreach (var authConfig in AuthProviders)
            {
                if (string.Compare(authConfig.Provider, provider,
                    StringComparison.InvariantCultureIgnoreCase) == 0)
                    return authConfig;
            }

            return null;
        }

        /// <summary>Initialises this object.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="sessionFactory">The session factory.</param>
        /// <param name="authProviders"> A variable-length parameters list containing authentication providers.</param>
        public static void Init(Func<IAuthSession> sessionFactory, params IAuthProvider[] authProviders)
        {
            EndpointHost.AssertTestConfig();

            if (authProviders.Length == 0)
                throw new ArgumentNullException("authProviders");

            DefaultOAuthProvider = authProviders[0].Provider;
            DefaultOAuthRealm = authProviders[0].AuthRealm;

            AuthProviders = authProviders;
            if (sessionFactory != null)
                CurrentSessionFactory = sessionFactory;
        }

        private void AssertAuthProviders()
        {
            if (AuthProviders == null || AuthProviders.Length == 0)
                throw new ConfigurationErrorsException("No OAuth providers have been registered in your AppHost.");
        }

        /// <summary>Options the given request.</summary>
        ///
        /// <param name="request">.</param>
        public virtual void Options(Auth request) {}

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">.</param>
        ///
        /// <returns>An object.</returns>
        public virtual object Get(Auth request)
        {
            return Post(request);
        }

        /// <summary>Post this message.</summary>
        ///
        /// <exception cref="NotFound"> Thrown when a not found error condition occurs.</exception>
        /// <exception cref="HttpError">Thrown when a HTTP error error condition occurs.</exception>
        ///
        /// <param name="request">.</param>
        ///
        /// <returns>An object.</returns>
        public virtual object Post(Auth request)
        {
            AssertAuthProviders();

            if (ValidateFn != null)
            {
                var validationResponse = ValidateFn(this, HttpMethods.Get, request);
                if (validationResponse != null) return validationResponse;
            }

            if (request.RememberMe.HasValue)
            {
                var opt = request.RememberMe.GetValueOrDefault(false)
                    ? SessionOptions.Permanent
                    : SessionOptions.Temporary;

                base.RequestContext.Get<IHttpResponse>()
                    .AddSessionOptions(base.RequestContext.Get<IHttpRequest>(), opt);
            }

            var provider = request.provider ?? AuthProviders[0].Provider;
            var oAuthConfig = GetAuthProvider(provider);
            if (oAuthConfig == null)
                throw HttpError.NotFound("No configuration was added for OAuth provider '{0}'".Fmt(provider));

            if (request.provider == LogoutAction)
                return oAuthConfig.Logout(this, request);

            var session = this.GetSession();

            var isHtml = base.RequestContext.ResponseContentType.MatchesContentType(ContentType.Html);
            try
            {
                var response = Authenticate(request, provider, session, oAuthConfig);

                // The above Authenticate call may end an existing session and create a new one so we need
                // to refresh the current session reference.
                session = this.GetSession();

                var referrerUrl = request.Continue
                    ?? session.ReferrerUrl
                    ?? this.RequestContext.GetHeader("Referer")
                    ?? oAuthConfig.CallbackUrl;

                var alreadyAuthenticated = response == null;
                response = response ?? new AuthResponse {
                    UserName = session.UserAuthName,
                    SessionId = session.Id,
                    ReferrerUrl = referrerUrl,
                };

                if (isHtml)
                {
                    if (alreadyAuthenticated)
                        return this.Redirect(referrerUrl.AddHashParam("s", "0"));

                    if (!(response is IHttpResult) && !String.IsNullOrEmpty(referrerUrl))
                    {
                        return new HttpResult(response) {
                            Location = referrerUrl
                        };
                    }
                }

                return response;
            }
            catch (HttpError ex)
            {
                var errorReferrerUrl = this.RequestContext.GetHeader("Referer");
                if (isHtml && errorReferrerUrl != null)
                {
                    errorReferrerUrl = errorReferrerUrl.SetQueryParam("error", ex.Message);
                    return HttpResult.Redirect(errorReferrerUrl);
                }

                throw;
            }
        }

        /// <summary>
        /// Public API entry point to authenticate via code
        /// </summary>
        /// <param name="request"></param>
        /// <returns>null; if already autenticated otherwise a populated instance of AuthResponse</returns>
        public virtual AuthResponse Authenticate(Auth request)
        {
            //Remove HTML Content-Type to avoid auth providers issuing browser re-directs
            ((HttpRequestContext)this.RequestContext).ResponseContentType = ContentType.PlainText;

            var provider = request.provider ?? AuthProviders[0].Provider;
            var oAuthConfig = GetAuthProvider(provider);
            if (oAuthConfig == null)
                throw HttpError.NotFound("No configuration was added for OAuth provider '{0}'".Fmt(provider));

            if (request.provider == LogoutAction)
                return oAuthConfig.Logout(this, request) as AuthResponse;

            var result = Authenticate(request, provider, this.GetSession(), oAuthConfig);
            var httpError = result as HttpError;
            if (httpError != null)
                throw httpError;

            return result as AuthResponse;
        }

        /// <summary>
        /// The specified <paramref name="session"/> may change as a side-effect of this method. If
        /// subsequent code relies on current <see cref="IAuthSession"/> data be sure to reload
        /// the session istance via <see cref="ServiceExtensions.GetSession(NServiceKit.ServiceInterface.IServiceBase,bool)"/>.
        /// </summary>
        private object Authenticate(Auth request, string provider, IAuthSession session, IAuthProvider oAuthConfig)
        {
            object response = null;
            if (!oAuthConfig.IsAuthorized(session, session.GetOAuthTokens(provider), request))
            {
                response = oAuthConfig.Authenticate(this, session, request);
            }
            return response;
        }

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">.</param>
        ///
        /// <returns>An object.</returns>
        public virtual object Delete(Auth request)
        {
            if (ValidateFn != null)
            {
                var response = ValidateFn(this, HttpMethods.Delete, request);
                if (response != null) return response;
            }

            this.RemoveSession();

            return new AuthResponse();
        }
    }

}

