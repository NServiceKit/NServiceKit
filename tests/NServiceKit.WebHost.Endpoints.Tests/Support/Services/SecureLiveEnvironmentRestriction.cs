using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A secure live environment restriction.</summary>
	[Restrict(EndpointAttributes.External | EndpointAttributes.Secure | EndpointAttributes.HttpPost | EndpointAttributes.Xml)]
	[DataContract]
	public class SecureLiveEnvironmentRestriction { }

    /// <summary>A secure live environment restriction response.</summary>
	[DataContract]
	public class SecureLiveEnvironmentRestrictionResponse { }

    /// <summary>A secure live environment restriction service.</summary>
	public class SecureLiveEnvironmentRestrictionService
		: TestServiceBase<SecureLiveEnvironmentRestriction>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(SecureLiveEnvironmentRestriction request)
		{
			return new SecureLiveEnvironmentRestrictionResponse();
		}
	}
}