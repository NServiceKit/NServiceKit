using NServiceKit.IntegrationTests.ServiceModel;

namespace NServiceKit.IntegrationTests.ServiceInterface
{
	public class LocalSubnetRestrictionService
		: TestServiceBase<LocalSubnetRestriction>
	{
		protected override object Run(LocalSubnetRestriction request)
		{
			return new LocalSubnetRestrictionResponse();
		}
	}
}