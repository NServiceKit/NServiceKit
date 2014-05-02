using System;
using System.Net;
using System.Web;
using NServiceKit.Common;
using NServiceKit.Configuration;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceModel;
using NServiceKit.Text;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>
    /// Create a Facebook App at: https://developers.facebook.com/apps
    /// The Callback URL for your app should match the CallbackUrl provided.
    /// </summary>
    public class FacebookAuthProvider : OAuthProvider
    {
        /// <summary>The name.</summary>
        public const string Name = "facebook";
        /// <summary>The realm.</summary>
        public static string Realm = "https://graph.facebook.com/";
        /// <summary>URL of the pre authentication.</summary>
        public static string PreAuthUrl = "https://www.facebook.com/dialog/oauth";

        /// <summary>Gets or sets the identifier of the application.</summary>
        ///
        /// <value>The identifier of the application.</value>
        public string AppId { get; set; }

        /// <summary>Gets or sets the application secret.</summary>
        ///
        /// <value>The application secret.</value>
        public string AppSecret { get; set; }

        /// <summary>Gets or sets the permissions.</summary>
        ///
        /// <value>The permissions.</value>
        public string[] Permissions { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.FacebookAuthProvider class.</summary>
        ///
        /// <param name="appSettings">The application settings.</param>
        public FacebookAuthProvider(IResourceManager appSettings)
            : base(appSettings, Realm, Name, "AppId", "AppSecret")
        {
            this.AppId = appSettings.GetString("oauth.facebook.AppId");
            this.AppSecret = appSettings.GetString("oauth.facebook.AppSecret");
            this.Permissions = appSettings.Get("oauth.facebook.Permissions", new string[0]);
        }

        /// <summary>The entry point for all AuthProvider providers. Runs inside the AuthService so exceptions are treated normally. Overridable so you can provide your own Auth implementation.</summary>
        ///
        /// <param name="authService">.</param>
        /// <param name="session">    .</param>
        /// <param name="request">    .</param>
        ///
        /// <returns>An object.</returns>
        public override object Authenticate(IServiceBase authService, IAuthSession session, Auth request)
        {
            var tokens = Init(authService, ref session, request);

            var error = authService.RequestContext.Get<IHttpRequest>().QueryString["error"];
            var hasError = !error.IsNullOrEmpty();
            if (hasError)
            {
                Log.Error("Facebook error callback. {0}".Fmt(authService.RequestContext.Get<IHttpRequest>().QueryString));
                return authService.Redirect(session.ReferrerUrl);
            }

            var code = authService.RequestContext.Get<IHttpRequest>().QueryString["code"];
            var isPreAuthCallback = !code.IsNullOrEmpty();
            if (!isPreAuthCallback)
            {
                var preAuthUrl = PreAuthUrl + "?client_id={0}&redirect_uri={1}&scope={2}"
                    .Fmt(AppId, this.CallbackUrl.UrlEncode(), string.Join(",", Permissions));

                authService.SaveSession(session, SessionExpiry);
                return authService.Redirect(preAuthUrl);
            }

            var accessTokenUrl = this.AccessTokenUrl + "?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}"
                .Fmt(AppId, this.CallbackUrl.UrlEncode(), AppSecret, code);

            try
            {
                var contents = accessTokenUrl.GetStringFromUrl();
                var authInfo = HttpUtility.ParseQueryString(contents);
                tokens.AccessTokenSecret = authInfo["access_token"];
                session.IsAuthenticated = true;
                authService.SaveSession(session, SessionExpiry);
                OnAuthenticated(authService, session, tokens, authInfo.ToDictionary());

                //Haz access!
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

            //Shouldn't get here
            return authService.Redirect(session.ReferrerUrl.AddHashParam("f", "Unknown"));
        }

        /// <summary>Loads user authentication information.</summary>
        ///
        /// <param name="userSession">The user session.</param>
        /// <param name="tokens">     The tokens.</param>
        /// <param name="authInfo">   Information describing the authentication.</param>
        protected override void LoadUserAuthInfo(AuthUserSession userSession, IOAuthTokens tokens, System.Collections.Generic.Dictionary<string, string> authInfo)
        {
            try
            {
                var json = AuthHttpGateway.DownloadFacebookUserInfo(tokens.AccessTokenSecret);
                var obj = JsonObject.Parse(json);
                tokens.UserId = obj.Get("id");
                tokens.UserName = obj.Get("username");
                tokens.DisplayName = obj.Get("name");
                tokens.FirstName = obj.Get("first_name");
                tokens.LastName = obj.Get("last_name");
                tokens.Email = obj.Get("email");

                LoadUserOAuthProvider(userSession, tokens);
            }
            catch (Exception ex)
            {
                Log.Error("Could not retrieve facebook user info for '{0}'".Fmt(tokens.DisplayName), ex);
            }
        }

        /// <summary>Loads user o authentication provider.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        /// <param name="tokens">     The tokens.</param>
        public override void LoadUserOAuthProvider(IAuthSession authSession, IOAuthTokens tokens)
        {
            var userSession = authSession as AuthUserSession;
            if (userSession == null) return;

            userSession.FacebookUserId = tokens.UserId ?? userSession.FacebookUserId;
            userSession.FacebookUserName = tokens.UserName ?? userSession.FacebookUserName;
            userSession.DisplayName = tokens.DisplayName ?? userSession.DisplayName;
            userSession.FirstName = tokens.FirstName ?? userSession.FirstName;
            userSession.LastName = tokens.LastName ?? userSession.LastName;
            userSession.PrimaryEmail = tokens.Email ?? userSession.PrimaryEmail ?? userSession.Email;
        }
    }
}