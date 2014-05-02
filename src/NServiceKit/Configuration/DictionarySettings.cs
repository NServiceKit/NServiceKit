using System.Collections.Generic;

namespace NServiceKit.Configuration
{
    /// <summary>A dictionary settings.</summary>
    public class DictionarySettings : AppSettingsBase, ISettings
    {
        private readonly Dictionary<string, string> map;

        /// <summary>Initializes a new instance of the NServiceKit.Configuration.DictionarySettings class.</summary>
        ///
        /// <param name="map">The map.</param>
        public DictionarySettings(Dictionary<string, string> map=null)
        {
            this.map = map ?? new Dictionary<string, string>();
            settings = this;
        }

        /// <summary>
        /// Provides a common interface for Settings providers such as ConfigurationManager or Azure's RoleEnvironment. The only requirement is that if the implementation cannot find the specified key, the
        /// return value must be null.
        /// </summary>
        ///
        /// <param name="key">The key for the setting.</param>
        ///
        /// <returns>The string value of the specified key, or null if the key was invalid.</returns>
        public string Get(string key)
        {
            string value;
            return map.TryGetValue(key, out value) ? value : null;
        }
    }
}