using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.IntegrationTests.ServiceModel
{
	[Service(EndpointAttributes.LocalSubnet)]
	[DataContract]
	public class LocalSubnetRestriction { }

	[DataContract]
	public class LocalSubnetRestrictionResponse { }
}