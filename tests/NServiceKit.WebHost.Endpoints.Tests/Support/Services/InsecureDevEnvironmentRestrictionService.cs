using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
	[Restrict(EndpointAttributes.InternalNetworkAccess | EndpointAttributes.InSecure | EndpointAttributes.HttpPost | EndpointAttributes.Xml)]
	[DataContract]
	public class InSecureDevEnvironmentRestriction { }

	[DataContract]
	public class InsecureDevEnvironmentRestrictionResponse { }

	public class InsecureDevEnvironmentRestrictionService
		: TestServiceBase<InSecureDevEnvironmentRestriction>
	{
		protected override object Run(InSecureDevEnvironmentRestriction request)
		{
			return new InsecureDevEnvironmentRestrictionResponse();
		}
	}

}