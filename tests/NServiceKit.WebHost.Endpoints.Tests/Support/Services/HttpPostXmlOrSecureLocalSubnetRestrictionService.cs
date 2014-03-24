using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
	[Restrict(EndpointAttributes.LocalSubnet | EndpointAttributes.Secure, EndpointAttributes.HttpPost | EndpointAttributes.Xml)]
	[DataContract]
	public class HttpPostXmlOrSecureLocalSubnetRestriction { }

	[DataContract]
	public class HttpPostXmlOrSecureLocalSubnetRestrictionResponse { }

	public class HttpPostXmlOrSecureLocalSubnetRestrictionService
		: TestServiceBase<HttpPostXmlOrSecureLocalSubnetRestriction>
	{
		protected override object Run(HttpPostXmlOrSecureLocalSubnetRestriction request)
		{
			return new HttpPostXmlOrSecureLocalSubnetRestrictionResponse();
		}
	}
}