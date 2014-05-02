using System.Runtime.Serialization;
using NServiceKit.WebHost.Endpoints.Tests.Support.Types;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
    /// <summary>A get customer.</summary>
	[DataContract]
	public class GetCustomer
	{
        /// <summary>Gets or sets the identifier of the customer.</summary>
        ///
        /// <value>The identifier of the customer.</value>
		[DataMember]
		public long CustomerId { get; set; }
	}

    /// <summary>A get customer response.</summary>
	[DataContract]
	public class GetCustomerResponse
	{
        /// <summary>Gets or sets the customer.</summary>
        ///
        /// <value>The customer.</value>
		[DataMember]
		public Customer Customer { get; set; }
	}
}
