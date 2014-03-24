using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
	[Restrict(EndpointAttributes.Localhost)]
	[DataContract]
	public class LocalhostRestriction { }

	[DataContract]
	public class LocalhostRestrictionResponse { }

	public class LocalhostRestrictionService
		: TestServiceBase<LocalhostRestriction>
	{
		protected override object Run(LocalhostRestriction request)
		{
			return new LocalhostRestrictionResponse();
		}
	}

}
