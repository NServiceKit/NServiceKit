using System.Collections.Generic;
using System.Runtime.Serialization;
using NServiceKit.WebHost.Endpoints.Tests.Support.Types;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
	[DataContract]
	public class GetCustomers
	{
		public GetCustomers()
		{
			this.CustomerIds = new List<long>();
		}

		[DataMember]
		public List<long> CustomerIds { get; set; }
	}

	[DataContract]
	public class GetCustomersResponse
	{
		[DataMember]
		public List<Customer> Customers { get; set; }
	}
}