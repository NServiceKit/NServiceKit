using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>An in secure live environment restriction.</summary>
	[Restrict(EndpointAttributes.External | EndpointAttributes.InSecure | EndpointAttributes.HttpPost | EndpointAttributes.Xml)]
	[DataContract]
	public class InSecureLiveEnvironmentRestriction { }

    /// <summary>An in secure live environment restriction response.</summary>
	[DataContract]
	public class InSecureLiveEnvironmentRestrictionResponse { }

    /// <summary>An in secure live environment restriction service.</summary>
	public class InSecureLiveEnvironmentRestrictionService
		: TestServiceBase<InSecureLiveEnvironmentRestriction>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(InSecureLiveEnvironmentRestriction request)
		{
			return new InSecureLiveEnvironmentRestrictionResponse();
		}
	}
}