using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
	[Restrict(AccessTo = EndpointAttributes.InternalNetworkAccess)]
	[DataContract]
	public class InternalRestriction { }

	[DataContract]
	public class IntranetRestrictionResponse { }

	public class InternalRestrictionService
		: TestServiceBase<InternalRestriction>
	{
		protected override object Run(InternalRestriction request)
		{
			return new IntranetRestrictionResponse();
		}
	}

}