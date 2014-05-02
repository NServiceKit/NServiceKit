using NServiceKit.Configuration;

namespace NServiceKit.Authentication.OpenId
{
    /// <summary>A yahoo open identifier o authentication provider.</summary>
    public class YahooOpenIdOAuthProvider : OpenIdOAuthProvider
    {
        /// <summary>The name.</summary>
        public const string Name = "YahooOpenId";
        /// <summary>The realm.</summary>
        public static string Realm = "https://me.yahoo.com";

        /// <summary>Initializes a new instance of the NServiceKit.Authentication.OpenId.YahooOpenIdOAuthProvider class.</summary>
        ///
        /// <param name="appSettings">The application settings.</param>
        public YahooOpenIdOAuthProvider(IResourceManager appSettings)
            : base(appSettings, Name, Realm) { }
    }
}