using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.IntegrationTests.ServiceModel
{
	[Service(EndpointAttributes.Localhost)]
	[DataContract]
	public class LocalhostRestriction { }

	[DataContract]
	public class LocalhostRestrictionResponse { }
}