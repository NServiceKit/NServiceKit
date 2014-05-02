using System.Runtime.Serialization;

namespace NServiceKit.ServiceModel.Tests.DataContracts
{
	[DataContract(Namespace = "http://schemas.NServiceKit.net/types/")]
	public class Customer 
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the store.</summary>
        ///
        /// <value>The identifier of the store.</value>
		public int StoreId { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
		public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
		public string LastName { get; set; }

        /// <summary>Gets or sets the email.</summary>
        ///
        /// <value>The email.</value>
		public string Email { get; set; }
	}
}