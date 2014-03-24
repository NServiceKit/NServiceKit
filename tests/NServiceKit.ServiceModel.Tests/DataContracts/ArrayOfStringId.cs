using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceModel.Tests.DataContracts
{
	[CollectionDataContract(Namespace = "http://schemas.NServiceKit.net/types/", ItemName = "Id")]
	public class ArrayOfStringId : List<string>
	{
		public ArrayOfStringId() { }
		public ArrayOfStringId(IEnumerable<string> collection) : base(collection) { }
	}
}