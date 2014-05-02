using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>The throws argument null.</summary>
	[Route("/throwsargumentnull")]
	[DataContract]
	public class ThrowsArgumentNull
	{
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		[DataMember]
		public string Value { get; set; }
	}

    /// <summary>The throws argument null response.</summary>
	[DataContract]
	public class ThrowsArgumentNullResponse
		: IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.ThrowsArgumentNullResponse class.</summary>
		public ThrowsArgumentNullResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>The throws argument null service.</summary>
	public class ThrowsArgumentNullService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(ThrowsArgumentNull request)
		{
			throw new ArgumentNullException("Name");
		}
	}
}