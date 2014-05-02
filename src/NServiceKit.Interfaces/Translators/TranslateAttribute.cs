using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceKit.Translators
{
	/// <summary>
	/// This instructs the generator tool to generate translator methods for the types supplied.
	/// A {TypeName}.generated.cs partial class will be generated that contains the methods required
	/// to generate to and from that type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TranslateAttribute : Attribute
	{
        /// <summary>Gets or sets source method prefix.</summary>
        ///
        /// <value>The source method prefix.</value>
		public string SourceMethodPrefix { get; set; }

        /// <summary>Gets or sets target method prefix.</summary>
        ///
        /// <value>The target method prefix.</value>
		public string TargetMethodPrefix { get; set; }

        /// <summary>Gets or sets the type of the source.</summary>
        ///
        /// <value>The type of the source.</value>
		public Type SourceType { get; set; }

        /// <summary>Gets or sets the type of the target.</summary>
        ///
        /// <value>The type of the target.</value>
		public Type TargetType { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Translators.TranslateAttribute class.</summary>
        ///
        /// <param name="targetType">Type of the target.</param>
		public TranslateAttribute(Type targetType) 
			: this(null, targetType) {}

        /// <summary>Initializes a new instance of the NServiceKit.Translators.TranslateAttribute class.</summary>
        ///
        /// <param name="sourceExtensionPrefix">Source extension prefix.</param>
        /// <param name="targetType">           Type of the target.</param>
        /// <param name="targetExtensionPrefix">Target extension prefix.</param>
		public TranslateAttribute(string sourceExtensionPrefix, Type targetType, string targetExtensionPrefix)
			: this(null, sourceExtensionPrefix, targetType, targetExtensionPrefix) { }

        /// <summary>Initializes a new instance of the NServiceKit.Translators.TranslateAttribute class.</summary>
        ///
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="targetType">Type of the target.</param>
		protected TranslateAttribute(Type sourceType, Type targetType)
		{
			this.SourceType = sourceType;
			this.TargetType = targetType;
		}

        /// <summary>Initializes a new instance of the NServiceKit.Translators.TranslateAttribute class.</summary>
        ///
        /// <param name="sourceType">           Type of the source.</param>
        /// <param name="sourceExtensionPrefix">Source extension prefix.</param>
        /// <param name="targetType">           Type of the target.</param>
        /// <param name="targetExtensionPrefix">Target extension prefix.</param>
		protected TranslateAttribute(Type sourceType, string sourceExtensionPrefix, Type targetType, string targetExtensionPrefix)
		{
			this.SourceType = sourceType;
			this.SourceMethodPrefix = sourceExtensionPrefix;
			this.TargetType = targetType;
			this.TargetMethodPrefix = targetExtensionPrefix;
		}
	}
}
