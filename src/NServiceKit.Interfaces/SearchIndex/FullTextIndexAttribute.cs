using System;

namespace NServiceKit.SearchIndex
{
    /// <summary>Bitfield of flags for specifying FullTextIndexAttribute.</summary>
	[Flags]
	public enum FullTextIndexAttribute
	{
        /// <summary>A binary constant representing the is default flag.</summary>
		IsDefault = 1,

        /// <summary>A binary constant representing the is key flag.</summary>
		IsKey = 2,

        /// <summary>A binary constant representing the no index flag.</summary>
		NoIndex = 4,

        /// <summary>A binary constant representing the index tokenized flag.</summary>
		IndexTokenized = 8,

        /// <summary>A binary constant representing the index un tokenized flag.</summary>
		IndexUnTokenized = 16,

        /// <summary>A binary constant representing the no store flag.</summary>
		NoStore = 32,

        /// <summary>A binary constant representing the store compressed flag.</summary>
		StoreCompressed = 64,

        /// <summary>A binary constant representing the store uncompressed flag.</summary>
		StoreUncompressed = 128,
	}
}