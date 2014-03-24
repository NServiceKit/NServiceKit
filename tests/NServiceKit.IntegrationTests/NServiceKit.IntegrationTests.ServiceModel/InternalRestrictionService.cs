using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.IntegrationTests.ServiceModel
{
	[Service(EndpointAttributes.InternalNetworkAccess)]
	[DataContract]
	public class InternalRestriction { }

	[DataContract]
	public class IntranetRestrictionResponse { }
}