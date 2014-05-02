using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>An in secure development environment restriction.</summary>
	[Restrict(EndpointAttributes.InternalNetworkAccess | EndpointAttributes.InSecure | EndpointAttributes.HttpPost | EndpointAttributes.Xml)]
	[DataContract]
	public class InSecureDevEnvironmentRestriction { }

    /// <summary>An insecure development environment restriction response.</summary>
	[DataContract]
	public class InsecureDevEnvironmentRestrictionResponse { }

    /// <summary>An insecure development environment restriction service.</summary>
	public class InsecureDevEnvironmentRestrictionService
		: TestServiceBase<InSecureDevEnvironmentRestriction>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(InSecureDevEnvironmentRestriction request)
		{
			return new InsecureDevEnvironmentRestrictionResponse();
		}
	}

}