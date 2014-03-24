using NServiceKit.WebHost.Endpoints.Support.Metadata;
using NServiceKit.WebHost.Endpoints.Support.Templates;

namespace NServiceKit.WebHost.Endpoints.Metadata
{
	public class Soap12WsdlMetadataHandler : WsdlMetadataHandlerBase
	{
		protected override WsdlTemplateBase GetWsdlTemplate()
		{
			return new Soap12WsdlTemplate();
		}
	}
}