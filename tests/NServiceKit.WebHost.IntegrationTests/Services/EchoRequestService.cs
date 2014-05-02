using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>An echo request.</summary>
	[DataContract]
	[Route("/echo/{Id}/{String}")]
	public class EchoRequest
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		[DataMember]
		public int Id { get; set; }

        /// <summary>Gets or sets the string.</summary>
        ///
        /// <value>The string.</value>
		[DataMember]
		public string String { get; set; }

        /// <summary>Gets or sets the long.</summary>
        ///
        /// <value>The long.</value>
		[DataMember]
		public long Long { get; set; }

        /// <summary>Gets or sets a unique identifier.</summary>
        ///
        /// <value>The identifier of the unique.</value>
		[DataMember]
		public Guid Guid { get; set; }

        /// <summary>Gets or sets a value indicating whether the. </summary>
        ///
        /// <value>true if , false if not.</value>
		[DataMember]
		public bool Bool { get; set; }

        /// <summary>Gets or sets the date time.</summary>
        ///
        /// <value>The date time.</value>
		[DataMember]
		public DateTime DateTime { get; set; }

        /// <summary>Gets or sets the double.</summary>
        ///
        /// <value>The double.</value>
		[DataMember]
		public double Double { get; set; }
	}

    /// <summary>An echo request response.</summary>
	[DataContract]
	public class EchoRequestResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>An echo request service.</summary>
	public class EchoRequestService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(EchoRequest request)
		{
			return request;
		}
	}
}