namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A SOAP metadata configuration.</summary>
	public class SoapMetadataConfig : MetadataConfig
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.SoapMetadataConfig class.</summary>
        ///
        /// <param name="format">            Describes the format to use.</param>
        /// <param name="name">              The name.</param>
        /// <param name="syncReplyUri">      URI of the synchronise reply.</param>
        /// <param name="asyncOneWayUri">    URI of the asynchronous one way.</param>
        /// <param name="defaultMetadataUri">The default metadata URI.</param>
        /// <param name="wsdlMetadataUri">   The wsdl metadata URI.</param>
        public SoapMetadataConfig(string format, string name, string syncReplyUri, string asyncOneWayUri, string defaultMetadataUri, string wsdlMetadataUri)
            : base(format, name, syncReplyUri, asyncOneWayUri, defaultMetadataUri)
		{
			WsdlMetadataUri = wsdlMetadataUri;
		}

        /// <summary>Gets or sets URI of the wsdl metadata.</summary>
        ///
        /// <value>The wsdl metadata URI.</value>
		public string WsdlMetadataUri { get; set; }
	}
}