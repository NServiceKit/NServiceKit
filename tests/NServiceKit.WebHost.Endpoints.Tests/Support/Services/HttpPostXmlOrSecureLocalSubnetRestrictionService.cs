using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A HTTP post XML or secure local subnet restriction.</summary>
	[Restrict(EndpointAttributes.LocalSubnet | EndpointAttributes.Secure, EndpointAttributes.HttpPost | EndpointAttributes.Xml)]
	[DataContract]
	public class HttpPostXmlOrSecureLocalSubnetRestriction { }

    /// <summary>A HTTP post XML or secure local subnet restriction response.</summary>
	[DataContract]
	public class HttpPostXmlOrSecureLocalSubnetRestrictionResponse { }

    /// <summary>A HTTP post XML or secure local subnet restriction service.</summary>
	public class HttpPostXmlOrSecureLocalSubnetRestrictionService
		: TestServiceBase<HttpPostXmlOrSecureLocalSubnetRestriction>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(HttpPostXmlOrSecureLocalSubnetRestriction request)
		{
			return new HttpPostXmlOrSecureLocalSubnetRestrictionResponse();
		}
	}
}