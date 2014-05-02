using System;

namespace NServiceKit.Translators
{
	/// <summary>
	/// This instructs the generator tool to generate translator extension methods for the types supplied.
	/// A {TypeName}.generated.cs static class will be generated that contains the extension methods required
	/// to generate to and from that type.
	/// 
	/// The source type is what the type the attribute is decorated on which can only be resolved at runtime.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class TranslateExtensionAttribute : TranslateAttribute
	{
        /// <summary>Initializes a new instance of the NServiceKit.Translators.TranslateExtensionAttribute class.</summary>
        ///
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="targetType">Type of the target.</param>
		public TranslateExtensionAttribute(Type sourceType, Type targetType)
			: base(sourceType, targetType) {}

        /// <summary>Initializes a new instance of the NServiceKit.Translators.TranslateExtensionAttribute class.</summary>
        ///
        /// <param name="sourceType">           Type of the source.</param>
        /// <param name="sourceExtensionPrefix">Source extension prefix.</param>
        /// <param name="targetType">           Type of the target.</param>
        /// <param name="targetExtensionPrefix">Target extension prefix.</param>
		public TranslateExtensionAttribute(Type sourceType, string sourceExtensionPrefix, Type targetType, string targetExtensionPrefix)
			:base(sourceType, sourceExtensionPrefix, targetType, targetExtensionPrefix) {}
	}

}
