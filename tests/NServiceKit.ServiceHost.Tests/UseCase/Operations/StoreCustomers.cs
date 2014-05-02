using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceHost.Tests.UseCase.Operations
{
    /// <summary>A store customers.</summary>
	[DataContract]
	public class StoreCustomers
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.UseCase.Operations.StoreCustomers class.</summary>
		public StoreCustomers()
		{
			Customers = new List<Customer>();
		}

        /// <summary>Gets or sets the customers.</summary>
        ///
        /// <value>The customers.</value>
		[DataMember]
		public List<Customer> Customers { get; set; }
	}
}