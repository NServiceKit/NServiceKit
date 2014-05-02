using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceModel.Tests.DataContracts
{
	[CollectionDataContract(Namespace = "http://schemas.NServiceKit.net/types/", ItemName = "Id")]
	public class ArrayOfStringId : List<string>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Tests.DataContracts.ArrayOfStringId class.</summary>
		public ArrayOfStringId() { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Tests.DataContracts.ArrayOfStringId class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfStringId(IEnumerable<string> collection) : base(collection) { }
	}
}