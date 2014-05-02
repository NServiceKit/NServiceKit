using System;
using System.Collections.Generic;

namespace NServiceKit.DataAnnotations
{
    /// <summary>Attribute for composite index.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class CompositeIndexAttribute : Attribute
	{
        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.CompositeIndexAttribute class.</summary>
		public CompositeIndexAttribute()
		{
			this.FieldNames = new List<string>();
		}

        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.CompositeIndexAttribute class.</summary>
        ///
        /// <param name="fieldNames">A list of names of the fields.</param>
		public CompositeIndexAttribute(params string[] fieldNames)
		{
			this.FieldNames = new List<string>(fieldNames);
		}

        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.CompositeIndexAttribute class.</summary>
        ///
        /// <param name="unique">    true if unique, false if not.</param>
        /// <param name="fieldNames">A list of names of the fields.</param>
		public CompositeIndexAttribute(bool unique, params string[] fieldNames)
		{
			this.Unique = unique;
			this.FieldNames = new List<string>(fieldNames);
		}

        /// <summary>Gets or sets a list of names of the fields.</summary>
        ///
        /// <value>A list of names of the fields.</value>
		public List<string> FieldNames { get; set; }

        /// <summary>Gets or sets a value indicating whether the unique.</summary>
        ///
        /// <value>true if unique, false if not.</value>
		public bool Unique { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
	    public string Name { get; set; }
	}
}