using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A HTTP post XML and secure local subnet restriction.</summary>
	[Restrict(EndpointAttributes.LocalSubnet | EndpointAttributes.Secure | EndpointAttributes.HttpPost | EndpointAttributes.Xml)]
	[DataContract]
	public class HttpPostXmlAndSecureLocalSubnetRestriction { }

    /// <summary>A HTTP post XML and secure local subnet restriction response.</summary>
	[DataContract]
	public class HttpPostXmlAndSecureLocalSubnetRestrictionResponse { }

    /// <summary>A HTTP post XML and secure local subnet restriction service.</summary>
	public class HttpPostXmlAndSecureLocalSubnetRestrictionService
		: TestServiceBase<HttpPostXmlAndSecureLocalSubnetRestriction>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(HttpPostXmlAndSecureLocalSubnetRestriction request)
		{
			return new HttpPostXmlAndSecureLocalSubnetRestrictionResponse();
		}
	}

}