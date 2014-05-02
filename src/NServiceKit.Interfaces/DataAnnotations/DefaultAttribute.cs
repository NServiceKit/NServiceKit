using System;

namespace NServiceKit.DataAnnotations
{
    /// <summary>Attribute for default.</summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class DefaultAttribute : Attribute
	{
        /// <summary>Gets or sets the int value.</summary>
        ///
        /// <value>The int value.</value>
		public int IntValue { get; set; }

        /// <summary>Gets or sets the double value.</summary>
        ///
        /// <value>The double value.</value>
		public double DoubleValue { get; set; }

        /// <summary>Gets or sets the default type.</summary>
        ///
        /// <value>The default type.</value>
		public Type DefaultType { get; set; }

        /// <summary>Gets or sets the default value.</summary>
        ///
        /// <value>The default value.</value>
		public string DefaultValue { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.DefaultAttribute class.</summary>
        ///
        /// <param name="intValue">The int value.</param>
		public DefaultAttribute(int intValue)
		{
			this.IntValue = intValue;
		}

        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.DefaultAttribute class.</summary>
        ///
        /// <param name="doubleValue">The double value.</param>
		public DefaultAttribute(double doubleValue)
		{
			this.DoubleValue = doubleValue;
		}

        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.DefaultAttribute class.</summary>
        ///
        /// <param name="defaultType"> The default type.</param>
        /// <param name="defaultValue">The default value.</param>
		public DefaultAttribute(Type defaultType, string defaultValue)
		{
			this.DefaultValue = defaultValue;
			this.DefaultType = defaultType;
		}
	}
}