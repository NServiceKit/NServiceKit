using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.IntegrationTests.ServiceModel
{
	[Service(EndpointAttributes.Secure | EndpointAttributes.LocalSubnet)]
	[DataContract]
	public class SecureLocalSubnetRestriction { }

	[DataContract]
	public class SecureLocalSubnetRestrictionResponse { }
}