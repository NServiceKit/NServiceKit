using NServiceKit.ServiceInterface.ServiceModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceModel.Tests.DataContracts.Operations
{
	[DataContract(Namespace = "http://schemas.NServiceKit.net/types/")]
	public class GetCustomersResponse : IExtensibleDataObject
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Tests.DataContracts.Operations.GetCustomersResponse class.</summary>
		public GetCustomersResponse()
		{
			this.Version = 100;
			this.Customers = new List<Customer>();
		}

        /// <summary>Gets or sets the customers.</summary>
        ///
        /// <value>The customers.</value>

        /// <summary>Gets or sets the customers.</summary>
        ///
        /// <value>The customers.</value>
		[DataMember]
		public List<Customer> Customers { get; set; }

        /// <summary>Gets or sets the version.</summary>
        ///
        /// <value>The version.</value>
		[DataMember]
		public int Version { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }

        /// <summary>Gets or sets the properties.</summary>
        ///
        /// <value>The properties.</value>
		[DataMember]
		public Properties Properties { get; set; }

        /// <summary>Gets or sets the structure that contains extra data.</summary>
        ///
        /// <value>An <see cref="T:System.Runtime.Serialization.ExtensionDataObject" /> that contains data that is not recognized as belonging to the data contract.</value>
		public ExtensionDataObject ExtensionData { get; set; }
	}
}