namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A configuration.</summary>
	public class Config
	{
        /// <summary>URI of the absolute base.</summary>
        public const string AbsoluteBaseUri = "http://localhost:50001/";
        /// <summary>URI of the service kit base.</summary>
        public const string NServiceKitBaseUri = AbsoluteBaseUri + "api";
	}
}