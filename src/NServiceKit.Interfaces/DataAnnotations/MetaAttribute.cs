using System;

namespace NServiceKit.DataAnnotations
{
    /// <summary>
    /// Decorate any type or property with adhoc info
    /// </summary>
    public class MetaAttribute : Attribute
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
        public string Value { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.DataAnnotations.MetaAttribute class.</summary>
        ///
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        public MetaAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}