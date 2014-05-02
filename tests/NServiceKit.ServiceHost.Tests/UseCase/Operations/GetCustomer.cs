using System.Runtime.Serialization;

namespace NServiceKit.ServiceHost.Tests.UseCase.Operations
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

    /// <summary>A customer.</summary>
	[DataContract]
	public class Customer
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		[DataMember]
		public long Id { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
		[DataMember]
		public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
		[DataMember]
		public string LastName { get; set; }
	}
}