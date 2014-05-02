using System;
using System.Collections.Generic;
using System.Text;
using NServiceKit.SearchIndex;

namespace NServiceKit.SearchIndex
{
    /// <summary>Attribute for full text index field.</summary>
	public class FullTextIndexFieldAttribute : Attribute
	{
        /// <summary>Gets the field attributes.</summary>
        ///
        /// <value>The field attributes.</value>
		public FullTextIndexAttribute FieldAttributes { get; private set; }

        /// <summary>Gets or sets the full pathname of the member file.</summary>
        ///
        /// <value>The full pathname of the member file.</value>
		public string MemberPath { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.SearchIndex.FullTextIndexFieldAttribute class.</summary>
		public FullTextIndexFieldAttribute() 
				:this(FullTextIndexAttribute.StoreUncompressed | FullTextIndexAttribute.IndexTokenized)
		{}

        /// <summary>Initializes a new instance of the NServiceKit.SearchIndex.FullTextIndexFieldAttribute class.</summary>
        ///
        /// <param name="fieldAttributes">The field attributes.</param>
		public FullTextIndexFieldAttribute(FullTextIndexAttribute fieldAttributes)
		{
			this.FieldAttributes = fieldAttributes;
		}

        /// <summary>Initializes a new instance of the NServiceKit.SearchIndex.FullTextIndexFieldAttribute class.</summary>
        ///
        /// <param name="fieldAttributes">The field attributes.</param>
        /// <param name="memberPath">     Full pathname of the member file.</param>
		public FullTextIndexFieldAttribute(FullTextIndexAttribute fieldAttributes, string memberPath)
				: this(fieldAttributes)
		{
			this.MemberPath = memberPath;
		}

        /// <summary>Initializes a new instance of the NServiceKit.SearchIndex.FullTextIndexFieldAttribute class.</summary>
        ///
        /// <param name="memberTypePropertyName">Name of the member type property.</param>
		public FullTextIndexFieldAttribute(string memberTypePropertyName)
				: this()
		{
			this.MemberPath = memberTypePropertyName;
		}

	}
}