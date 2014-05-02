using System.Collections.Generic;

namespace NServiceKit.Configuration
{
    /// <summary>Interface for resource manager.</summary>
	public interface IResourceManager
	{
        /// <summary>Gets a string.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>The string.</returns>
		string GetString(string name);

        /// <summary>Gets a list.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The list.</returns>
		IList<string> GetList(string key);

        /// <summary>Gets a dictionary.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The dictionary.</returns>
		IDictionary<string, string> GetDictionary(string key);

        /// <summary>Gets.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="name">        The name.</param>
        /// <param name="defaultValue">The default value.</param>
        ///
        /// <returns>A T.</returns>
		T Get<T>(string name, T defaultValue);
	}
}