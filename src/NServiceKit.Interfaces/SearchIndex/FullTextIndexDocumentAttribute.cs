using System;

namespace NServiceKit.SearchIndex
{
    /// <summary>Attribute for full text index document.</summary>
	public class FullTextIndexDocumentAttribute : Attribute
	{
        /// <summary>Gets or sets the type of for.</summary>
        ///
        /// <value>The type of for.</value>
		public Type ForType { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.SearchIndex.FullTextIndexDocumentAttribute class.</summary>
		public FullTextIndexDocumentAttribute()
		{}

        /// <summary>Initializes a new instance of the NServiceKit.SearchIndex.FullTextIndexDocumentAttribute class.</summary>
        ///
        /// <param name="forType">Type of for.</param>
		public FullTextIndexDocumentAttribute(Type forType)
		{
			this.ForType = forType;
		}
	}
}