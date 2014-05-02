using System.Configuration;

namespace NServiceKit.Configuration
{
    /// <summary>
    /// More familiar name for the new crowd.
    /// </summary>
    public class AppSettings : AppSettingsBase
    {
        private class ConfigurationManagerWrapper : ISettings
        {
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
                return ConfigurationManager.AppSettings[key];
            }
        }

        /// <summary>Initializes a new instance of the NServiceKit.Configuration.AppSettings class.</summary>
        public AppSettings() : base(new ConfigurationManagerWrapper()) {}

        /// <summary>
        /// Returns string if exists, otherwise null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetString(string name) //Keeping backwards compatible
        {
            return base.GetNullableString(name); 
        }
    }

    /// <summary>Manager for configuration resources.</summary>
    public class ConfigurationResourceManager : AppSettings {}
}