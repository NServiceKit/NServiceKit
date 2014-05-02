using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A localhost restriction.</summary>
	[Restrict(EndpointAttributes.Localhost)]
	[DataContract]
	public class LocalhostRestriction { }

    /// <summary>A localhost restriction response.</summary>
	[DataContract]
	public class LocalhostRestrictionResponse { }

    /// <summary>A localhost restriction service.</summary>
	public class LocalhostRestrictionService
		: TestServiceBase<LocalhostRestriction>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(LocalhostRestriction request)
		{
			return new LocalhostRestrictionResponse();
		}
	}

}
