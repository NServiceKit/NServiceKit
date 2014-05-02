using System;

namespace NServiceKit.Translators
{
	/// <summary>
	/// This changes the default behaviour for the 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class TranslateMemberAttribute : Attribute
	{
        /// <summary>Gets or sets the name of the property.</summary>
        ///
        /// <value>The name of the property.</value>
		public string PropertyName { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Translators.TranslateMemberAttribute class.</summary>
        ///
        /// <param name="toPropertyName">Name of to property.</param>
		public TranslateMemberAttribute(string toPropertyName)
		{
			this.PropertyName = toPropertyName;
		}
	}
}