using NServiceKit.CacheAccess;
using NServiceKit.Configuration;

namespace NServiceKit.LogicFacade
{
    /// <summary>Interface for application context.</summary>
	public interface IApplicationContext
	{
        /// <summary>Gets the factory.</summary>
        ///
        /// <value>The factory.</value>
		IFactoryProvider Factory { get; }

        /// <summary>Gets the get.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
		T Get<T>() where T : class;

        /// <summary>Gets the cache.</summary>
        ///
        /// <value>The cache.</value>
		ICacheClient Cache { get; }

        /// <summary>Gets the resources.</summary>
        ///
        /// <value>The resources.</value>
		IResourceManager Resources { get; }
	}
}