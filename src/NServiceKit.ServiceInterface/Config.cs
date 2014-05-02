namespace NServiceKit.ServiceInterface
{
    /// <summary>A configuration.</summary>
    public class Config
    {
        /// <summary>
        /// Would've preferred to use [assembly: ContractNamespace] attribute but it is not supported in Mono
        /// </summary>
        //public const string DefaultNamespace = "http://schemas.sericestack.net/examples/types";
        public const string DefaultNamespace = "http://schemas.NServiceKit.net/types";

        /// <summary>URI of the service kit base.</summary>
        public const string NServiceKitBaseUri = "http://localhost:20000";
        /// <summary>URI of the absolute base.</summary>
        public const string AbsoluteBaseUri = NServiceKitBaseUri + "/";
    }
}