using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
	[Restrict(EndpointAttributes.Secure | EndpointAttributes.LocalSubnet)]
	[DataContract]
	public class SecureLocalSubnetRestriction { }

	[DataContract]
	public class SecureLocalSubnetRestrictionResponse { }

	public class SecureLocalSubnetRestrictionService
		: TestServiceBase<SecureLocalSubnetRestriction>
	{
		protected override object Run(SecureLocalSubnetRestriction request)
		{
			return new SecureLocalSubnetRestrictionResponse();
		}
	}

}