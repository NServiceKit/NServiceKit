using NServiceKit.ServiceInterface.ServiceModel;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceModel.Tests.DataContracts.Operations
{
	[DataContract(Namespace = "http://schemas.NServiceKit.net/types/")]
	public class GetCustomers : IExtensibleDataObject
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Tests.DataContracts.Operations.GetCustomers class.</summary>
		public GetCustomers()
		{
			this.CustomerIds = new ArrayOfIntId();
			this.Version = 100;
		}

        /// <summary>Gets or sets a list of identifiers of the customers.</summary>
        ///
        /// <value>A list of identifiers of the customers.</value>

        /// <summary>Gets or sets a list of identifiers of the customers.</summary>
        ///
        /// <value>A list of identifiers of the customers.</value>
		[DataMember]
		public ArrayOfIntId CustomerIds { get; set; }

        /// <summary>Gets or sets the version.</summary>
        ///
        /// <value>The version.</value>
		[DataMember]
		public int Version { get; set; }

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