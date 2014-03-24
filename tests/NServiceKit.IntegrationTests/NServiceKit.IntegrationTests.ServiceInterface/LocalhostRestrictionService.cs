using NServiceKit.IntegrationTests.ServiceModel;

namespace NServiceKit.IntegrationTests.ServiceInterface
{
	public class LocalhostRestrictionService
		: TestServiceBase<LocalhostRestriction>
	{
		protected override object Run(LocalhostRestriction request)
		{
			return new LocalhostRestrictionResponse();
		}
	}
}