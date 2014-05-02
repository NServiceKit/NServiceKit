using NServiceKit.Common.Web;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A service endpoints metadata configuration.</summary>
    public class ServiceEndpointsMetadataConfig
    {
        private ServiceEndpointsMetadataConfig() { }

        /// <summary>
        /// Changes the links for the NServiceKit/metadata page
        /// </summary>
        /// <param name="NServiceKitHandlerPrefix"></param>
        /// <returns></returns>
        public static ServiceEndpointsMetadataConfig Create(string NServiceKitHandlerPrefix)
        {
            var config = new MetadataConfig("{0}", "{0}", "/{0}/reply", "/{0}/oneway", "/{0}/metadata");
            return new ServiceEndpointsMetadataConfig {
                DefaultMetadataUri = "/metadata",
                Soap11 = new SoapMetadataConfig("soap11", "SOAP 1.1", "/soap11", "/soap11", "/soap11/metadata", "soap11"),
                Soap12 = new SoapMetadataConfig("soap12", "SOAP 1.2", "/soap12", "/soap12", "/soap12/metadata", "soap12"),
                Xml = config.Create("xml"),
                Json = config.Create("json"),
                Jsv = config.Create("jsv"),
                Custom = config
            };
        }

        /// <summary>Gets or sets the default metadata URI.</summary>
        ///
        /// <value>The default metadata URI.</value>
        public string DefaultMetadataUri { get; set; }

        /// <summary>Gets or sets the SOAP 11.</summary>
        ///
        /// <value>The SOAP 11.</value>
        public SoapMetadataConfig Soap11 { get; set; }

        /// <summary>Gets or sets the SOAP 12.</summary>
        ///
        /// <value>The SOAP 12.</value>
        public SoapMetadataConfig Soap12 { get; set; }

        /// <summary>Gets or sets the XML.</summary>
        ///
        /// <value>The XML.</value>
        public MetadataConfig Xml { get; set; }

        /// <summary>Gets or sets the JSON.</summary>
        ///
        /// <value>The JSON.</value>
        public MetadataConfig Json { get; set; }

        /// <summary>Gets or sets the jsv.</summary>
        ///
        /// <value>The jsv.</value>
        public MetadataConfig Jsv { get; set; }

        /// <summary>Gets or sets the custom.</summary>
        ///
        /// <value>The custom.</value>
        public MetadataConfig Custom { get; set; }

        /// <summary>Gets endpoint configuration.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>The endpoint configuration.</returns>
        public MetadataConfig GetEndpointConfig(string contentType)
        {
            switch (contentType)
            {
                case ContentType.Soap11:
                    return this.Soap11;
                case ContentType.Soap12:
                    return this.Soap12;
                case ContentType.Xml:
                    return this.Xml;
                case ContentType.Json:
                    return this.Json;
                case ContentType.Jsv:
                    return this.Jsv;
            }

            var format = ContentType.GetContentFormat(contentType);
            return Custom.Create(format);
        }
    }
}