using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceModel.Tests.DataContracts
{
	[CollectionDataContract(Namespace = "http://schemas.NServiceKit.net/types/", ItemName = "Id")]
	public class ArrayOfGuidId : List<Guid>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Tests.DataContracts.ArrayOfGuidId class.</summary>
		public ArrayOfGuidId() { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Tests.DataContracts.ArrayOfGuidId class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfGuidId(IEnumerable<Guid> collection) : base(collection) { }
	}
}