using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A local subnet restriction.</summary>
	[Restrict(EndpointAttributes.LocalSubnet)]
	[DataContract]
	public class LocalSubnetRestriction { }

    /// <summary>A local subnet restriction response.</summary>
	[DataContract]
	public class LocalSubnetRestrictionResponse { }

    /// <summary>A local subnet restriction service.</summary>
	public class LocalSubnetRestrictionService
		: TestServiceBase<LocalSubnetRestriction>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(LocalSubnetRestriction request)
		{
			return new LocalSubnetRestrictionResponse();
		}
	}

}