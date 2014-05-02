using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceModel.Tests.DataContracts
{
	[CollectionDataContract(Namespace = "http://schemas.NServiceKit.net/types/", ItemName = "Id")]
	public class ArrayOfIntId : List<int>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Tests.DataContracts.ArrayOfIntId class.</summary>
		public ArrayOfIntId() { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Tests.DataContracts.ArrayOfIntId class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfIntId(IEnumerable<int> collection) : base(collection) { }
	}
}