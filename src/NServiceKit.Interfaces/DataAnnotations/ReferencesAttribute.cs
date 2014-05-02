using System;

namespace NServiceKit.DataAnnotations
{
    /// <summary>Attribute for references.</summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
	public class ReferencesAttribute : Attribute
	{
        /// <summary>Gets or sets the type.</summary>
        ///
        /// <value>The type.</value>
		public Type Type { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.ReferencesAttribute class.</summary>
        ///
        /// <param name="type">The type.</param>
		public ReferencesAttribute(Type type)
		{
			this.Type = type;
		}
	}
}