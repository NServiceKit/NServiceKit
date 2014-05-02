using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceInterface.ServiceModel
{
    /// <summary>A property.</summary>
	[DataContract]
	public class Property
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		public string Value { get; set; }
	}

    /// <summary>A properties.</summary>
	[CollectionDataContract(ItemName = "Property")]
	public class Properties : List<Property>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.Properties class.</summary>
		public Properties() { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.Properties class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public Properties(IEnumerable<Property> collection) : base(collection) { }
	}
}