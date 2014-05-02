using System;
using System.Collections.Generic;
using System.Net;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Configuration;
using NServiceKit.Logging;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>An authentication provider.</summary>
    public abstract class AuthProvider : IAuthProvider
    {
        /// <summary>The log.</summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(AuthProvider));

        /// <summary>Gets or sets the session expiry.</summary>
        ///
        /// <value>The session expiry.</value>
        public TimeSpan? SessionExpiry { get; set; }

        /// <summary>Gets or sets the authentication realm.</summary>
        ///
        /// <value>The authentication realm.</value>
        public string AuthRealm { get; set; }

        /// <summary>Gets or sets the provider.</summary>
        ///
        /// <value>The provider.</value>
        public string Provider { get; set; }

        /// <summary>Gets or sets URL of the callback.</summary>
        ///
        /// <value>The callback URL.</value>
        public string CallbackUrl { get; set; }

        /// <summary>Gets or sets URL of the redirect.</summary>
        ///
        /// <value>The redirect URL.</value>
        public string RedirectUrl { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.AuthProvider class.</summary>
        protected AuthProvider() { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.AuthProvider class.</summary>
        ///
        /// <param name="appSettings">  The application settings.</param>
        /// <param name="authRealm">    The authentication realm.</param>
        /// <param name="oAuthProvider">The authentication provider.</param>
        protected AuthProvider(IResourceManager appSettings, string authRealm, string oAuthProvider)
        {
            // Enhancement per https://github.com/NServiceKit/NServiceKit/issues/741
            this.AuthRealm = appSettings != null ? appSettings.Get("OAuthRealm", authRealm) : authRealm;

            this.Provider = oAuthProvider;
            if (appSettings != null)
            {
                this.CallbackUrl = appSettings.GetString("oauth.{0}.CallbackUrl".Fmt(oAuthProvider))
                    ?? FallbackConfig(appSettings.GetString("oauth.CallbackUrl"));
                this.RedirectUrl = appSettings.GetString("oauth.{0}.RedirectUrl".Fmt(oAuthProvider))
                    ?? FallbackConfig(appSettings.GetString("oauth.RedirectUrl"));
            }
            this.SessionExpiry = SessionFeature.DefaultSessionExpiry;
        }

        /// <summary>
        /// Allows specifying a global fallback config that if exists is formatted with the Provider as the first arg.
        /// E.g. this appSetting with the TwitterAuthProvider: 
        /// oauth.CallbackUrl="http://localhost:11001/auth/{0}"
        /// Would result in: 
        /// oauth.CallbackUrl="http://localhost:11001/auth/twitter"
        /// </summary>
        /// <returns></returns>
        protected string FallbackConfig(string fallback)
        {
            return fallback != null ? fallback.Fmt(Provider) : null;
        }

        /// <summary>
        /// Remove the Users Session
        /// </summary>
        /// <param name="service"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual object Logout(IServiceBase service, Auth request)
        {
            var session = service.GetSession();
            var referrerUrl = (request != null ? request.Continue : null)
                ?? session.ReferrerUrl
                ?? service.RequestContext.GetHeader("Referer")
                ?? this.CallbackUrl;

            session.OnLogout(service);

            service.RemoveSession();

            if (service.RequestContext.ResponseContentType == ContentType.Html && !String.IsNullOrEmpty(referrerUrl))
                return service.Redirect(referrerUrl.AddHashParam("s", "-1"));

            return new AuthResponse();
        }

        /// <summary>
        /// Saves the Auth Tokens for this request. Called in OnAuthenticated(). 
        /// Overrideable, the default behaviour is to call IUserAuthRepository.CreateOrMergeAuthSession().
        /// </summary>
        protected virtual void SaveUserAuth(IServiceBase authService, IAuthSession session, IUserAuthRepository authRepo, IOAuthTokens tokens)
        {
            if (authRepo == null) return;
            if (tokens != null)
            {
                session.UserAuthId = authRepo.CreateOrMergeAuthSession(session, tokens);
            }

            authRepo.LoadUserAuth(session, tokens);

            foreach (var oAuthToken in session.ProviderOAuthAccess)
            {
                var authProvider = AuthService.GetAuthProvider(oAuthToken.Provider);
                if (authProvider == null) continue;
                var userAuthProvider = authProvider as OAuthProvider;
                if (userAuthProvider != null)
                {
                    userAuthProvider.LoadUserOAuthProvider(session, oAuthToken);
                }
            }

            authRepo.SaveUserAuth(session);

            var httpRes = authService.RequestContext.Get<IHttpResponse>();
            if (httpRes != null)
            {
                httpRes.Cookies.AddPermanentCookie(HttpHeaders.XUserAuthId, session.UserAuthId);
            }
            OnSaveUserAuth(authService, session);
        }

        /// <summary>Executes the save user authentication action.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        public virtual void OnSaveUserAuth(IServiceBase authService, IAuthSession session) { }

        /// <summary>Executes the authenticated action.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        /// <param name="tokens">     The tokens.</param>
        /// <param name="authInfo">   Information describing the authentication.</param>
        public virtual void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            var userSession = session as AuthUserSession;
            if (userSession != null)
            {
                LoadUserAuthInfo(userSession, tokens, authInfo);
            }

            var authRepo = authService.TryResolve<IUserAuthRepository>();
            if (authRepo != null)
            {
                if (tokens != null)
                {
                    authInfo.ForEach((x, y) => tokens.Items[x] = y);
                    session.UserAuthId = authRepo.CreateOrMergeAuthSession(session, tokens);
                }
                //SaveUserAuth(authService, userSession, authRepo, tokens);
                
                authRepo.LoadUserAuth(session, tokens);

                foreach (var oAuthToken in session.ProviderOAuthAccess)
                {
                    var authProvider = AuthService.GetAuthProvider(oAuthToken.Provider);
                    if (authProvider == null) continue;
                    var userAuthProvider = authProvider as OAuthProvider;
                    if (userAuthProvider != null)
                    {
                        userAuthProvider.LoadUserOAuthProvider(session, oAuthToken);
                    }
                }
        
                var httpRes = authService.RequestContext.Get<IHttpResponse>();
                if (httpRes != null)
                {
                    httpRes.Cookies.AddPermanentCookie(HttpHeaders.XUserAuthId, session.UserAuthId);
                }
                
            }

            //OnSaveUserAuth(authService, session);
            authService.SaveSession(session, SessionExpiry);
            session.OnAuthenticated(authService, session, tokens, authInfo);
        }

        /// <summary>Loads user authentication information.</summary>
        ///
        /// <param name="userSession">The user session.</param>
        /// <param name="tokens">     The tokens.</param>
        /// <param name="authInfo">   Information describing the authentication.</param>
        protected virtual void LoadUserAuthInfo(AuthUserSession userSession, IOAuthTokens tokens, Dictionary<string, string> authInfo) { }

        /// <summary>Login matches session.</summary>
        ///
        /// <param name="session"> The session.</param>
        /// <param name="userName">Name of the user.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        protected static bool LoginMatchesSession(IAuthSession session, string userName)
        {
            if (userName == null) return false;
            var isEmail = userName.Contains("@");
            if (isEmail)
            {
                if (!userName.EqualsIgnoreCase(session.Email))
                    return false;
            }
            else
            {
                if (!userName.EqualsIgnoreCase(session.UserName))
                    return false;
            }
            return true;
        }

        /// <summary>Determine if the current session is already authenticated with this AuthProvider.</summary>
        ///
        /// <param name="session">The session.</param>
        /// <param name="tokens"> The tokens.</param>
        /// <param name="request">The request.</param>
        ///
        /// <returns>true if authorized, false if not.</returns>
        public abstract bool IsAuthorized(IAuthSession session, IOAuthTokens tokens, Auth request = null);

        /// <summary>The entry point for all AuthProvider providers. Runs inside the AuthService so exceptions are treated normally. Overridable so you can provide your own Auth implementation.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        /// <param name="request">    The request.</param>
        ///
        /// <returns>An object.</returns>
        public abstract object Authenticate(IServiceBase authService, IAuthSession session, Auth request);

        /// <summary>Executes the failed authentication action.</summary>
        ///
        /// <param name="session">The session.</param>
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        public virtual void OnFailedAuthentication(IAuthSession session, IHttpRequest httpReq, IHttpResponse httpRes)
        {
            httpRes.StatusCode = (int)HttpStatusCode.Unauthorized;
            httpRes.AddHeader(HttpHeaders.WwwAuthenticate, "{0} realm=\"{1}\"".Fmt(this.Provider, this.AuthRealm));
            httpRes.EndRequest();
        }

        /// <summary>Handles the failed authentication.</summary>
        ///
        /// <param name="authProvider">The authentication provider.</param>
        /// <param name="session">     The session.</param>
        /// <param name="httpReq">     The HTTP request.</param>
        /// <param name="httpRes">     The HTTP resource.</param>
        public static void HandleFailedAuth(IAuthProvider authProvider,
            IAuthSession session, IHttpRequest httpReq, IHttpResponse httpRes)
        {
            var baseAuthProvider = authProvider as AuthProvider;
            if (baseAuthProvider != null)
            {
                baseAuthProvider.OnFailedAuthentication(session, httpReq, httpRes);
                return;
            }

            httpRes.StatusCode = (int)HttpStatusCode.Unauthorized;
            httpRes.AddHeader(HttpHeaders.WwwAuthenticate, "{0} realm=\"{1}\""
                .Fmt(authProvider.Provider, authProvider.AuthRealm));

            httpRes.EndRequest();
        }
    }

    /// <summary>An authentication configuration extensions.</summary>
    public static class AuthConfigExtensions
    {
        /// <summary>An IAuthProvider extension method that query if 'authProvider' is authorized safe.</summary>
        ///
        /// <param name="authProvider">The authProvider to act on.</param>
        /// <param name="session">     The session.</param>
        /// <param name="tokens">      The tokens.</param>
        ///
        /// <returns>true if authorized safe, false if not.</returns>
        public static bool IsAuthorizedSafe(this IAuthProvider authProvider, IAuthSession session, IOAuthTokens tokens)
        {
            return authProvider != null && authProvider.IsAuthorized(session, tokens);
        }
    }

}

