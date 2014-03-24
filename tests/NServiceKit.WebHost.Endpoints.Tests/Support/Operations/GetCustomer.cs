using System.Runtime.Serialization;
using NServiceKit.WebHost.Endpoints.Tests.Support.Types;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
	[DataContract]
	public class GetCustomer
	{
		[DataMember]
		public long CustomerId { get; set; }
	}

	[DataContract]
	public class GetCustomerResponse
	{
		[DataMember]
		public Customer Customer { get; set; }
	}
}
