using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A secure local subnet restriction.</summary>
	[Restrict(EndpointAttributes.Secure | EndpointAttributes.LocalSubnet)]
	[DataContract]
	public class SecureLocalSubnetRestriction { }

    /// <summary>A secure local subnet restriction response.</summary>
	[DataContract]
	public class SecureLocalSubnetRestrictionResponse { }

    /// <summary>A secure local subnet restriction service.</summary>
	public class SecureLocalSubnetRestrictionService
		: TestServiceBase<SecureLocalSubnetRestriction>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(SecureLocalSubnetRestriction request)
		{
			return new SecureLocalSubnetRestrictionResponse();
		}
	}

}