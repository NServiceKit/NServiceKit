using NServiceKit.IntegrationTests.ServiceModel;

namespace NServiceKit.IntegrationTests.ServiceInterface
{
	public class HttpPostXmlAndSecureLocalSubnetRestrictionService
		: TestServiceBase<HttpPostXmlAndSecureLocalSubnetRestriction>
	{
		protected override object Run(HttpPostXmlAndSecureLocalSubnetRestriction request)
		{
			return new HttpPostXmlAndSecureLocalSubnetRestrictionResponse();
		}
	}
}