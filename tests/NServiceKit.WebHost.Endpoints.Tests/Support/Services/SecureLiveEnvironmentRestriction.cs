using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
	[Restrict(EndpointAttributes.External | EndpointAttributes.Secure | EndpointAttributes.HttpPost | EndpointAttributes.Xml)]
	[DataContract]
	public class SecureLiveEnvironmentRestriction { }

	[DataContract]
	public class SecureLiveEnvironmentRestrictionResponse { }

	public class SecureLiveEnvironmentRestrictionService
		: TestServiceBase<SecureLiveEnvironmentRestriction>
	{
		protected override object Run(SecureLiveEnvironmentRestriction request)
		{
			return new SecureLiveEnvironmentRestrictionResponse();
		}
	}
}