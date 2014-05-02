using System;

namespace NServiceKit.Configuration
{
    /// <summary>Interface for type factory.</summary>
	public interface ITypeFactory
	{
        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>The new instance.</returns>
		object CreateInstance(Type type);
	}
}