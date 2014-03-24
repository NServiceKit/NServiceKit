using NServiceKit.IntegrationTests.ServiceModel;

namespace NServiceKit.IntegrationTests.ServiceInterface
{
	public class InternalRestrictionService
		: TestServiceBase<InternalRestriction>
	{
		protected override object Run(InternalRestriction request)
		{
			return new IntranetRestrictionResponse();
		}
	}
}