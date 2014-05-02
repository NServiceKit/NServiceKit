using System;
using System.Collections.Generic;
using System.Configuration;
using NServiceKit.Text;

namespace NServiceKit.Configuration
{
    /// <summary>An application settings base.</summary>
    public class AppSettingsBase : IResourceManager
    {
        /// <summary>Options for controlling the operation.</summary>
        protected ISettings settings;
        const string ErrorAppsettingNotFound = "Unable to find App Setting: {0}";

        /// <summary>Initializes a new instance of the NServiceKit.Configuration.AppSettingsBase class.</summary>
        ///
        /// <param name="settings">Options for controlling the operation.</param>
        public AppSettingsBase(ISettings settings=null)
        {
            this.settings = settings;
        }

        /// <summary>Gets nullable string.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>The nullable string.</returns>
        public virtual string GetNullableString(string name)
        {
            return settings.Get(name);
        }

        /// <summary>Gets a string.</summary>
        ///
        /// <exception cref="ConfigurationErrorsException">Thrown when a Configuration Errors error condition occurs.</exception>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>The string.</returns>
        public virtual string GetString(string name)
        {
            var value = GetNullableString(name);
            if (value == null)
            {
                throw new ConfigurationErrorsException(String.Format(ErrorAppsettingNotFound, name));
            }

            return value;
        }

        /// <summary>Gets a list.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The list.</returns>
        public virtual IList<string> GetList(string key)
        {
            var value = GetString(key);
            return ConfigUtils.GetListFromAppSettingValue(value);
        }

        /// <summary>Gets a dictionary.</summary>
        ///
        /// <exception cref="ConfigurationErrorsException">Thrown when a Configuration Errors error condition occurs.</exception>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The dictionary.</returns>
        public virtual IDictionary<string, string> GetDictionary(string key)
        {
            var value = GetString(key);
            try
            {
                return ConfigUtils.GetDictionaryFromAppSettingValue(value);
            }
            catch (Exception ex)
            {
                var message =
                    string.Format(
                        "The {0} setting had an invalid dictionary format. The correct format is of type \"Key1:Value1,Key2:Value2\"",
                        key);
                throw new ConfigurationErrorsException(message, ex);
            }
        }

        /// <summary>Gets.</summary>
        ///
        /// <exception cref="ConfigurationErrorsException">Thrown when a Configuration Errors error condition occurs.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="name">        The name.</param>
        /// <param name="defaultValue">The default value.</param>
        ///
        /// <returns>A T.</returns>
        public virtual T Get<T>(string name, T defaultValue)
        {
            var stringValue = GetNullableString(name);

            T deserializedValue;
            try
            {
                deserializedValue = TypeSerializer.DeserializeFromString<T>(stringValue);
            }
            catch (Exception ex)
            {
                var message =
                   string.Format(
                       "The {0} setting had an invalid format. The value \"{1}\" could not be cast to type {2}",
                       name, stringValue, typeof(T).FullName);
                throw new ConfigurationErrorsException(message, ex);
            }

            return stringValue != null
                       ? deserializedValue
                       : defaultValue;
        }
    }
}