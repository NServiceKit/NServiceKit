using NServiceKit.IntegrationTests.ServiceModel;

namespace NServiceKit.IntegrationTests.ServiceInterface
{
	public class SecureLocalSubnetRestrictionService
		: TestServiceBase<SecureLocalSubnetRestriction>
	{
		protected override object Run(SecureLocalSubnetRestriction request)
		{
			return new SecureLocalSubnetRestrictionResponse();
		}
	}
}