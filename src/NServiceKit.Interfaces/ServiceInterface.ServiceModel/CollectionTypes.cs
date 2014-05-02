using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceInterface.ServiceModel
{
	/*
	 * Useful collection DTO's that provide pretty Xml output for collection types, e.g.
	 * 
	 * ArrayOfIntId Ids { get; set; }		
	 * ... =>
	 * 
	 * <Ids>
	 *   <Id>1</Id>
	 *   <Id>2</Id>
	 *   <Id>3</Id>
	 * <Ids>
	 */

    /// <content>An array of string.</content>
	[CollectionDataContract(ItemName = "String")]
	public partial class ArrayOfString : List<string>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfString class.</summary>
		public ArrayOfString()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfString class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfString(IEnumerable<string> collection) : base(collection) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfString class.</summary>
        ///
        /// <param name="args">A variable-length parameters list containing arguments.</param>
		public ArrayOfString(params string[] args) : base(args) { }
	}

    /// <content>An array of string identifier.</content>
	[CollectionDataContract(ItemName = "Id")]
	public partial class ArrayOfStringId : List<string>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfStringId class.</summary>
		public ArrayOfStringId()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfStringId class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfStringId(IEnumerable<string> collection) : base(collection) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfStringId class.</summary>
        ///
        /// <param name="args">A variable-length parameters list containing arguments.</param>
		public ArrayOfStringId(params string[] args) : base(args) { }
	}

    /// <content>An array of unique identifier.</content>
	[CollectionDataContract(ItemName = "Guid")]
	public partial class ArrayOfGuid : List<Guid>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfGuid class.</summary>
		public ArrayOfGuid()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfGuid class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfGuid(IEnumerable<Guid> collection) : base(collection) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfGuid class.</summary>
        ///
        /// <param name="args">A variable-length parameters list containing arguments.</param>
		public ArrayOfGuid(params Guid[] args) : base(args) { }
	}

    /// <content>An array of unique identifier.</content>
	[CollectionDataContract(ItemName = "Id")]
	public partial class ArrayOfGuidId : List<Guid>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfGuidId class.</summary>
		public ArrayOfGuidId()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfGuidId class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfGuidId(IEnumerable<Guid> collection) : base(collection) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfGuidId class.</summary>
        ///
        /// <param name="args">A variable-length parameters list containing arguments.</param>
		public ArrayOfGuidId(params Guid[] args) : base(args) { }
	}

    /// <content>An array of long.</content>
	[CollectionDataContract(ItemName = "Long")]
	public partial class ArrayOfLong : List<long>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfLong class.</summary>
		public ArrayOfLong()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfLong class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfLong(IEnumerable<long> collection) : base(collection) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfLong class.</summary>
        ///
        /// <param name="args">A variable-length parameters list containing arguments.</param>
		public ArrayOfLong(params long[] args) : base(args) { }
	}

    /// <content>An array of long identifier.</content>
	[CollectionDataContract(ItemName = "Id")]
	public partial class ArrayOfLongId : List<long>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfLongId class.</summary>
		public ArrayOfLongId()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfLongId class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfLongId(IEnumerable<long> collection) : base(collection) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfLongId class.</summary>
        ///
        /// <param name="args">A variable-length parameters list containing arguments.</param>
		public ArrayOfLongId(params long[] args) : base(args) { }
	}

    /// <content>An array of int.</content>
	[CollectionDataContract(ItemName = "Int")]
	public partial class ArrayOfInt : List<int>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfInt class.</summary>
		public ArrayOfInt()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfInt class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfInt(IEnumerable<int> collection) : base(collection) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfInt class.</summary>
        ///
        /// <param name="args">A variable-length parameters list containing arguments.</param>
		public ArrayOfInt(params int[] args) : base(args) { }
	}

    /// <content>An array of int identifier.</content>
	[CollectionDataContract(ItemName = "Id")]
	public partial class ArrayOfIntId : List<int>
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfIntId class.</summary>
		public ArrayOfIntId ()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfIntId class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public ArrayOfIntId(IEnumerable<int> collection) : base(collection) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceModel.ArrayOfIntId class.</summary>
        ///
        /// <param name="args">A variable-length parameters list containing arguments.</param>
		public ArrayOfIntId(params int[] args) : base(args) { }
	}

}