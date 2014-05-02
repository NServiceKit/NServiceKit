using System.Collections.Generic;
using System.Runtime.Serialization;
using NServiceKit.WebHost.Endpoints.Tests.Support.Types;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
    /// <summary>A get customers.</summary>
	[DataContract]
	public class GetCustomers
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Operations.GetCustomers class.</summary>
		public GetCustomers()
		{
			this.CustomerIds = new List<long>();
		}

        /// <summary>Gets or sets a list of identifiers of the customers.</summary>
        ///
        /// <value>A list of identifiers of the customers.</value>
		[DataMember]
		public List<long> CustomerIds { get; set; }
	}

    /// <summary>A get customers response.</summary>
	[DataContract]
	public class GetCustomersResponse
	{
        /// <summary>Gets or sets the customers.</summary>
        ///
        /// <value>The customers.</value>
		[DataMember]
		public List<Customer> Customers { get; set; }
	}
}