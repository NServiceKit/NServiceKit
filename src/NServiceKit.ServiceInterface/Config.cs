namespace NServiceKit.ServiceInterface
{
    public class Config
    {
        /// <summary>
        /// Would've preferred to use [assembly: ContractNamespace] attribute but it is not supported in Mono
        /// </summary>
        //public const string DefaultNamespace = "http://schemas.sericestack.net/examples/types";
        public const string DefaultNamespace = "http://schemas.NServiceKit.net/types";

        public const string NServiceKitBaseUri = "http://localhost:20000";
        public const string AbsoluteBaseUri = NServiceKitBaseUri + "/";
    }
}