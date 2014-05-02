using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>An internal restriction.</summary>
	[Restrict(AccessTo = EndpointAttributes.InternalNetworkAccess)]
	[DataContract]
	public class InternalRestriction { }

    /// <summary>An intranet restriction response.</summary>
	[DataContract]
	public class IntranetRestrictionResponse { }

    /// <summary>An internal restriction service.</summary>
	public class InternalRestrictionService
		: TestServiceBase<InternalRestriction>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(InternalRestriction request)
		{
			return new IntranetRestrictionResponse();
		}
	}

}