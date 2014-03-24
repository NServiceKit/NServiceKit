using System;
using System.Collections.Generic;
using System.Text;
using NServiceKit.SearchIndex;

namespace NServiceKit.SearchIndex
{
	public class FullTextIndexFieldAttribute : Attribute
	{
		public FullTextIndexAttribute FieldAttributes { get; private set; }
		public string MemberPath { get; set; }

		public FullTextIndexFieldAttribute() 
				:this(FullTextIndexAttribute.StoreUncompressed | FullTextIndexAttribute.IndexTokenized)
		{}

		public FullTextIndexFieldAttribute(FullTextIndexAttribute fieldAttributes)
		{
			this.FieldAttributes = fieldAttributes;
		}

		public FullTextIndexFieldAttribute(FullTextIndexAttribute fieldAttributes, string memberPath)
				: this(fieldAttributes)
		{
			this.MemberPath = memberPath;
		}

		public FullTextIndexFieldAttribute(string memberTypePropertyName)
				: this()
		{
			this.MemberPath = memberTypePropertyName;
		}

	}
}