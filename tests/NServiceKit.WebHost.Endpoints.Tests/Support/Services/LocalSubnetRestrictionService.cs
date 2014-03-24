using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
	[Restrict(EndpointAttributes.LocalSubnet)]
	[DataContract]
	public class LocalSubnetRestriction { }

	[DataContract]
	public class LocalSubnetRestrictionResponse { }

	public class LocalSubnetRestrictionService
		: TestServiceBase<LocalSubnetRestriction>
	{
		protected override object Run(LocalSubnetRestriction request)
		{
			return new LocalSubnetRestrictionResponse();
		}
	}

}