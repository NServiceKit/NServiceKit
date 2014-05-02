using NServiceKit.WebHost.Endpoints.Support.Metadata;
using NServiceKit.WebHost.Endpoints.Support.Templates;

namespace NServiceKit.WebHost.Endpoints.Metadata
{
    /// <summary>A SOAP 12 wsdl metadata handler.</summary>
	public class Soap12WsdlMetadataHandler : WsdlMetadataHandlerBase
	{
        /// <summary>Gets wsdl template.</summary>
        ///
        /// <returns>The wsdl template.</returns>
		protected override WsdlTemplateBase GetWsdlTemplate()
		{
			return new Soap12WsdlTemplate();
		}
	}
}