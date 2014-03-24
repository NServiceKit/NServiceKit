using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.IntegrationTests.ServiceModel
{
	[Service(EndpointAttributes.LocalSubnet | EndpointAttributes.Secure | EndpointAttributes.HttpPost | EndpointAttributes.Xml)]
	[DataContract]
	public class HttpPostXmlAndSecureLocalSubnetRestriction { }

	[DataContract]
	public class HttpPostXmlAndSecureLocalSubnetRestrictionResponse { }
}