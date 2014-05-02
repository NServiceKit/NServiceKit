using NServiceKit.Configuration;

namespace NServiceKit.Authentication.OpenId
{
    /// <summary>A google open identifier o authentication provider.</summary>
    public class GoogleOpenIdOAuthProvider : OpenIdOAuthProvider
    {
        /// <summary>The name.</summary>
        public const string Name = "GoogleOpenId";
        /// <summary>The realm.</summary>
        public static string Realm = "https://www.google.com/accounts/o8/id";

        /// <summary>Initializes a new instance of the NServiceKit.Authentication.OpenId.GoogleOpenIdOAuthProvider class.</summary>
        ///
        /// <param name="appSettings">The application settings.</param>
        public GoogleOpenIdOAuthProvider(IResourceManager appSettings)
            : base(appSettings, Name, Realm) { }
    }
}