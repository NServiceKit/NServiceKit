using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceModel.Tests.DataContracts
{
	[CollectionDataContract(Namespace = "http://schemas.NServiceKit.net/types/", ItemName = "Id")]
	public class ArrayOfGuidId : List<Guid>
	{
		public ArrayOfGuidId() { }
		public ArrayOfGuidId(IEnumerable<Guid> collection) : base(collection) { }
	}
}