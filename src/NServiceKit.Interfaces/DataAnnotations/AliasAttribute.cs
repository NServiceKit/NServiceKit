using System;

namespace NServiceKit.DataAnnotations
{
    /// <summary>Attribute for alias.</summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
	public class AliasAttribute : Attribute
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.AliasAttribute class.</summary>
        ///
        /// <param name="name">The name.</param>
		public AliasAttribute(string name)
		{
			this.Name = name;
		}
	}
}