using System;
using System.Collections.Generic;
using NServiceKit.Common;
using NServiceKit.Configuration;
using NServiceKit.Text;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>
    /// Create an app at https://dev.twitter.com/apps to get your ConsumerKey and ConsumerSecret for your app.
    /// The Callback URL for your app should match the CallbackUrl provided.
    /// </summary>
    public class TwitterAuthProvider : OAuthProvider
    {
        /// <summary>The name.</summary>
        public const string Name = "twitter";
        /// <summary>The realm.</summary>
        public static string Realm = "https://api.twitter.com/";

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.TwitterAuthProvider class.</summary>
        ///
        /// <param name="appSettings">The application settings.</param>
        public TwitterAuthProvider(IResourceManager appSettings)
            : base(appSettings, Realm, Name) {}

        /// <summary>Loads user authentication information.</summary>
        ///
        /// <param name="userSession">The user session.</param>
        /// <param name="tokens">     The tokens.</param>
        /// <param name="authInfo">   Information describing the authentication.</param>
        protected override void LoadUserAuthInfo(AuthUserSession userSession, IOAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            if (authInfo.ContainsKey("user_id"))
                tokens.UserId = authInfo.GetValueOrDefault("user_id");

            if (authInfo.ContainsKey("screen_name"))
                tokens.UserName = authInfo.GetValueOrDefault("screen_name");

            try
            {
                if (tokens.UserId != null)
                {
                    var json = AuthHttpGateway.DownloadTwitterUserInfo(tokens.UserId);
                    var objs = JsonObject.ParseArray(json);
                    if (objs.Count > 0)
                    {
                        var obj = objs[0];
                        tokens.DisplayName = obj.Get("name");
                    }
                }

                LoadUserOAuthProvider(userSession, tokens);
            }
            catch (Exception ex)
            {
                Log.Error("Could not retrieve twitter user info for '{0}'".Fmt(userSession.TwitterUserId), ex);
            }
        }

        /// <summary>Loads user o authentication provider.</summary>
        ///
        /// <param name="authSession">The user session.</param>
        /// <param name="tokens">     The tokens.</param>
        public override void LoadUserOAuthProvider(IAuthSession authSession, IOAuthTokens tokens)
        {
            var userSession = authSession as AuthUserSession;
            if (userSession == null) return;
            
            userSession.TwitterUserId = tokens.UserId ?? userSession.TwitterUserId;
            userSession.TwitterScreenName = tokens.UserName ?? userSession.TwitterScreenName;
            userSession.DisplayName = tokens.DisplayName ?? userSession.DisplayName;
        }
    }
}