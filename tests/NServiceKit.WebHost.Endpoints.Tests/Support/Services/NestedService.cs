using System.Runtime.Serialization;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A nested.</summary>
	[DataContract]
	public class Nested { }
    /// <summary>A nested response.</summary>
	[DataContract]
	public class NestedResponse { }

    /// <summary>A nested service.</summary>
	public class NestedService
		: TestServiceBase<Nested>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(Nested request)
		{
			return new NestedResponse();
		}
	}

}