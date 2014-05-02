using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;

using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Configuration;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Authentication.OAuth2
{
    /// <summary>An authentication 2 provider.</summary>
    public abstract class OAuth2Provider : AuthProvider
    {
        /// <summary>Initializes a new instance of the NServiceKit.Authentication.OAuth2.OAuth2Provider class.</summary>
        ///
        /// <param name="appSettings">The application settings.</param>
        /// <param name="realm">      The realm.</param>
        /// <param name="provider">   The provider.</param>
        protected OAuth2Provider(IResourceManager appSettings, string realm, string provider)
            : base(appSettings, realm, provider)
        {
            this.ConsumerKey = appSettings.GetString("oauth.{0}.ConsumerKey".Fmt(provider))
                ?? FallbackConfig(appSettings.GetString("oauth.ConsumerKey"));
            this.ConsumerSecret = appSettings.GetString("oauth.{0}.ConsumerSecret".Fmt(provider))
                ?? FallbackConfig(appSettings.GetString("oauth.ConsumerSecret"));
            var scopes = appSettings.GetString("oauth.{0}.Scopes".Fmt(provider))
                ?? FallbackConfig(appSettings.GetString("oauth.Scopes")) ?? "";
            this.Scopes = scopes.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            this.RequestTokenUrl = appSettings.GetString("oauth.{0}.RequestTokenUrl".Fmt(provider))
                ?? FallbackConfig(appSettings.GetString("oauth.RequestTokenUrl"));
            this.AuthorizeUrl = appSettings.GetString("oauth.{0}.AuthorizeUrl".Fmt(provider))
                ?? FallbackConfig(appSettings.GetString("oauth.AuthorizeUrl"));
            this.AccessTokenUrl = appSettings.GetString("oauth.{0}.AccessTokenUrl".Fmt(provider))
                ?? FallbackConfig(appSettings.GetString("oauth.AccessTokenUrl"));
            this.UserProfileUrl = appSettings.GetString("oauth.{0}.UserProfileUrl".Fmt(provider))
                ?? FallbackConfig(appSettings.GetString("oauth.UserProfileUrl"));
        }

        /// <summary>Gets or sets URL of the access token.</summary>
        ///
        /// <value>The access token URL.</value>
        public string AccessTokenUrl { get; set; }

        /// <summary>Gets or sets the authentication HTTP gateway.</summary>
        ///
        /// <value>The authentication HTTP gateway.</value>
        public IAuthHttpGateway AuthHttpGateway { get; set; }

        /// <summary>Gets or sets URL of the authorize.</summary>
        ///
        /// <value>The authorize URL.</value>
        public string AuthorizeUrl { get; set; }

        /// <summary>Gets or sets the consumer key.</summary>
        ///
        /// <value>The consumer key.</value>
        public string ConsumerKey { get; set; }

        /// <summary>Gets or sets the consumer secret.</summary>
        ///
        /// <value>The consumer secret.</value>
        public string ConsumerSecret { get; set; }

        /// <summary>Gets or sets URL of the request token.</summary>
        ///
        /// <value>The request token URL.</value>
        public string RequestTokenUrl { get; set; }

        /// <summary>Gets or sets URL of the user profile.</summary>
        ///
        /// <value>The user profile URL.</value>
        public string UserProfileUrl { get; set; }

        /// <summary>Gets or sets the scopes.</summary>
        ///
        /// <value>The scopes.</value>
        protected string[] Scopes { get; set; }

        /// <summary>The entry point for all AuthProvider providers. Runs inside the AuthService so exceptions are treated normally. Overridable so you can provide your own Auth implementation.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        /// <param name="request">    The request.</param>
        ///
        /// <returns>An object.</returns>
        public override object Authenticate(IServiceBase authService, IAuthSession session, Auth request)
        {
            var tokens = this.Init(authService, ref session, request);

            var authServer = new AuthorizationServerDescription { AuthorizationEndpoint = new Uri(this.AuthorizeUrl), TokenEndpoint = new Uri(this.AccessTokenUrl) };
            var authClient = new WebServerClient(authServer, this.ConsumerKey) {
                ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(this.ConsumerSecret),
            };

            var authState = authClient.ProcessUserAuthorization();
            if (authState == null)
            {
                try
                {
                    var authReq = authClient.PrepareRequestUserAuthorization(this.Scopes, new Uri(this.CallbackUrl));
                    var authContentType = authReq.Headers[HttpHeaders.ContentType];
                    var httpResult = new HttpResult(authReq.ResponseStream, authContentType) { StatusCode = authReq.Status, StatusDescription = "Moved Temporarily" };
                    foreach (string header in authReq.Headers)
                    {
                        httpResult.Headers[header] = authReq.Headers[header];
                    }

                    foreach (string name in authReq.Cookies)
                    {
                        var cookie = authReq.Cookies[name];

                        if (cookie != null)
                        {
                            httpResult.SetSessionCookie(name, cookie.Value, cookie.Path);
                        }
                    }

                    authService.SaveSession(session, this.SessionExpiry);
                    return httpResult;
                }
                catch (ProtocolException ex)
                {
                    Log.Error("Failed to login to {0}".Fmt(this.Provider), ex);
                    return authService.Redirect(session.ReferrerUrl.AddHashParam("f", "Unknown"));
                }
            }

            var accessToken = authState.AccessToken;
            if (accessToken != null)
            {
                try
                {
                    tokens.AccessToken = accessToken;
                    tokens.RefreshToken = authState.RefreshToken;
                    tokens.RefreshTokenExpiry = authState.AccessTokenExpirationUtc;
                    session.IsAuthenticated = true;
                    var authInfo = this.CreateAuthInfo(accessToken);
                    this.OnAuthenticated(authService, session, tokens, authInfo);
                    return authService.Redirect(session.ReferrerUrl.AddHashParam("s", "1"));
                }
                catch (WebException we)
                {
                    var statusCode = ((HttpWebResponse)we.Response).StatusCode;
                    if (statusCode == HttpStatusCode.BadRequest)
                    {
                        return authService.Redirect(session.ReferrerUrl.AddHashParam("f", "AccessTokenFailed"));
                    }
                }
            }

            return authService.Redirect(session.ReferrerUrl.AddHashParam("f", "RequestTokenFailed"));
        }

        /// <summary>Determine if the current session is already authenticated with this AuthProvider.</summary>
        ///
        /// <param name="session">The session.</param>
        /// <param name="tokens"> The tokens.</param>
        /// <param name="request">The request.</param>
        ///
        /// <returns>true if authorized, false if not.</returns>
        public override bool IsAuthorized(IAuthSession session, IOAuthTokens tokens, Auth request = null)
        {
            if (request != null)
            {
                if (!LoginMatchesSession(session, request.UserName))
                {
                    return false;
                }
            }

            return tokens != null && !string.IsNullOrEmpty(tokens.UserId);
        }

        /// <summary>Loads user o authentication provider.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        /// <param name="tokens">     The tokens.</param>
        public void LoadUserOAuthProvider(IAuthSession authSession, IOAuthTokens tokens)
        {
            var userSession = authSession as AuthUserSession;
            if (userSession == null)
            {
                return;
            }

            userSession.UserName = tokens.UserName ?? userSession.UserName;
            userSession.DisplayName = tokens.DisplayName ?? userSession.DisplayName;
            userSession.FirstName = tokens.FirstName ?? userSession.FirstName;
            userSession.LastName = tokens.LastName ?? userSession.LastName;
            userSession.PrimaryEmail = tokens.Email ?? userSession.PrimaryEmail ?? userSession.Email;
            userSession.Email = tokens.Email ?? userSession.PrimaryEmail ?? userSession.Email;
        }

        /// <summary>Creates authentication information.</summary>
        ///
        /// <param name="accessToken">The access token.</param>
        ///
        /// <returns>The new authentication information.</returns>
        protected abstract Dictionary<string, string> CreateAuthInfo(string accessToken);

        /// <summary>Loads user authentication information.</summary>
        ///
        /// <param name="userSession">The user session.</param>
        /// <param name="tokens">     The tokens.</param>
        /// <param name="authInfo">   Information describing the authentication.</param>
        protected override void LoadUserAuthInfo(AuthUserSession userSession, IOAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            try
            {
                tokens.UserId = authInfo["user_id"];
                tokens.UserName = authInfo["username"];
                tokens.DisplayName = authInfo["name"];
                tokens.FirstName = authInfo["first_name"];
                tokens.LastName = authInfo["last_name"];
                tokens.Email = authInfo["email"];
                userSession.UserAuthName = tokens.Email;

                this.LoadUserOAuthProvider(userSession, tokens);
            }
            catch (Exception ex)
            {
                Log.Error("Could not retrieve Profile info for '{0}'".Fmt(tokens.DisplayName), ex);
            }
        }

        /// <summary>Initialises this object.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        /// <param name="request">    The request.</param>
        ///
        /// <returns>The IOAuthTokens.</returns>
        protected IOAuthTokens Init(IServiceBase authService, ref IAuthSession session, Auth request)
        {
            var requestUri = authService.RequestContext.AbsoluteUri;
            if (this.CallbackUrl.IsNullOrEmpty())
            {
                this.CallbackUrl = requestUri;
            }

            if (session.ReferrerUrl.IsNullOrEmpty())
            {
                session.ReferrerUrl = (request != null ? request.Continue : null) ?? authService.RequestContext.GetHeader("Referer");
            }

            if (session.ReferrerUrl.IsNullOrEmpty() || session.ReferrerUrl.IndexOf("/auth", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                session.ReferrerUrl = this.RedirectUrl
                                      ?? NServiceKitHttpHandlerFactory.GetBaseUrl()
                                      ?? requestUri.Substring(0, requestUri.IndexOf("/", "https://".Length + 1, StringComparison.Ordinal));
            }

            var tokens = session.ProviderOAuthAccess.FirstOrDefault(x => x.Provider == this.Provider);
            if (tokens == null)
            {
                session.ProviderOAuthAccess.Add(tokens = new OAuthTokens { Provider = this.Provider });
            }

            return tokens;
        }
    }
}