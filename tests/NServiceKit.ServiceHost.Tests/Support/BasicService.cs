using System.Runtime.Serialization;

namespace NServiceKit.ServiceHost.Tests.Support
{
    /// <summary>A basic request.</summary>
	[DataContract]
	public class BasicRequest { }

    /// <summary>A basic request response.</summary>
	[DataContract]
	public class BasicRequestResponse { }

    /// <summary>A basic service.</summary>
	public class BasicService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A BasicRequestResponse.</returns>
        public BasicRequestResponse Any(BasicRequest request)
		{
			return new BasicRequestResponse();
		}
	}
}