using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A wild card request.</summary>
	[DataContract]
	[Route("/wildcard/{Id}/{Path}/{Action}")]
	[Route("/wildcard/{Id}/{RemainingPath*}")]
	public class WildCardRequest
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		[DataMember]
		public int Id { get; set; }

        /// <summary>Gets or sets the full pathname of the file.</summary>
        ///
        /// <value>The full pathname of the file.</value>
		[DataMember]
		public string Path { get; set; }

        /// <summary>Gets or sets the full pathname of the remaining file.</summary>
        ///
        /// <value>The full pathname of the remaining file.</value>
		[DataMember]
		public string RemainingPath { get; set; }

        /// <summary>Gets or sets the action.</summary>
        ///
        /// <value>The action.</value>
		[DataMember]
		public string Action { get; set; }
	}

    /// <summary>A wild card request response.</summary>
	[DataContract]
	public class WildCardRequestResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A basic wildcard.</summary>
    [Route("/path/{Tail*}")]
    public class BasicWildcard
    {
        /// <summary>Gets or sets the tail.</summary>
        ///
        /// <value>The tail.</value>
        public string Tail { get; set; }
    }
    
    /// <summary>A wild card request service.</summary>
	public class WildCardRequestService : NServiceKit.ServiceInterface.Service
	{
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(WildCardRequest request)
        {
            return request;
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(BasicWildcard request)
        {
            return request;
        }
    }
}