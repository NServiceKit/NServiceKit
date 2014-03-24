using System.Runtime.Serialization;
using NServiceKit.WebHost.Endpoints.Tests.Support.Types;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
	[DataContract]
	public class StoreCustomer
	{
		[DataMember]
		public Customer Customer { get; set; }
	}
}