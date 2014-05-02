using NServiceKit.Configuration;

namespace NServiceKit.Authentication.OpenId
{
    /// <summary>my open identifier o authentication provider.</summary>
    public class MyOpenIdOAuthProvider : OpenIdOAuthProvider
    {
        /// <summary>The name.</summary>
        public const string Name = "MyOpenId";
        /// <summary>The realm.</summary>
        public static string Realm = "http://www.myopenid.com";

        /// <summary>Initializes a new instance of the NServiceKit.Authentication.OpenId.MyOpenIdOAuthProvider class.</summary>
        ///
        /// <param name="appSettings">The application settings.</param>
        public MyOpenIdOAuthProvider(IResourceManager appSettings)
            : base(appSettings, Name, Realm) { }
    }
}