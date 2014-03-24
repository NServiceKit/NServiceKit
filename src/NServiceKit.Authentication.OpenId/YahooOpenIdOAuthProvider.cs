using NServiceKit.Configuration;

namespace NServiceKit.Authentication.OpenId
{
    public class YahooOpenIdOAuthProvider : OpenIdOAuthProvider
    {
        public const string Name = "YahooOpenId";
        public static string Realm = "https://me.yahoo.com";

        public YahooOpenIdOAuthProvider(IResourceManager appSettings)
            : base(appSettings, Name, Realm) { }
    }
}