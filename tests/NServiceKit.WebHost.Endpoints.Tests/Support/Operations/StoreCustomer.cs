using System.Runtime.Serialization;
using NServiceKit.WebHost.Endpoints.Tests.Support.Types;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
    /// <summary>A store customer.</summary>
	[DataContract]
	public class StoreCustomer
	{
        /// <summary>Gets or sets the customer.</summary>
        ///
        /// <value>The customer.</value>
		[DataMember]
		public Customer Customer { get; set; }
	}
}