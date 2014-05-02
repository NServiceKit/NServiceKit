using System;

namespace NServiceKit.DataAnnotations
{
    /// <summary>Attribute for index.</summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
	public class IndexAttribute : Attribute
	{
        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.IndexAttribute class.</summary>
		public IndexAttribute()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.IndexAttribute class.</summary>
        ///
        /// <param name="unique">true if unique, false if not.</param>
		public IndexAttribute(bool unique)
		{
			Unique = unique;
		}

        /// <summary>Gets or sets a value indicating whether the unique.</summary>
        ///
        /// <value>true if unique, false if not.</value>
		public bool Unique { get; set; }
	}
}