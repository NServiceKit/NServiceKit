using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{

    /// <summary>A secure development environment restriction.</summary>
	[Restrict(EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Secure | EndpointAttributes.HttpPost | EndpointAttributes.Xml)]
	[DataContract]
	public class SecureDevEnvironmentRestriction { }

    /// <summary>A secure development environment restriction response.</summary>
	[DataContract]
	public class SecureDevEnvironmentRestrictionResponse { }

    /// <summary>A secure development environment restriction service.</summary>
	public class SecureDevEnvironmentRestrictionService
		: TestServiceBase<SecureDevEnvironmentRestriction>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(SecureDevEnvironmentRestriction request)
		{
			return new SecureDevEnvironmentRestrictionResponse();
		}
	}

}