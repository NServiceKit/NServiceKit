using System;

namespace NServiceKit.Configuration
{
    /// <summary>Interface for factory provider.</summary>
	public interface IFactoryProvider 
		: IContainerAdapter, IDisposable
	{
        /// <summary>Registers this object.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="provider">The provider.</param>
		void Register<T>(T provider);

        /// <summary>Resolves.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="name">The name.</param>
        ///
        /// <returns>A T.</returns>
		T Resolve<T>(string name);

        /// <summary>Resolve optional.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="name">        The name.</param>
        /// <param name="defaultValue">The default value.</param>
        ///
        /// <returns>A T.</returns>
		T ResolveOptional<T>(string name, T defaultValue);

        /// <summary>Creates a new T.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="name">The name.</param>
        ///
        /// <returns>A T.</returns>
		T Create<T>(string name);
	}
}