using System;
using System.Globalization;

namespace NServiceKit.Html
{
	internal static class Error
	{
        /// <summary>View data dictionary wrong t model type.</summary>
        ///
        /// <param name="valueType">Type of the value.</param>
        /// <param name="modelType">Type of the model.</param>
        ///
        /// <returns>An InvalidOperationException.</returns>
		public static InvalidOperationException ViewDataDictionary_WrongTModelType(Type valueType, Type modelType)
		{
			string message = String.Format(CultureInfo.CurrentCulture, MvcResources.ViewDataDictionary_WrongTModelType,
				valueType, modelType);
			return new InvalidOperationException(message);
		}

        /// <summary>View data dictionary model cannot be null.</summary>
        ///
        /// <param name="modelType">Type of the model.</param>
        ///
        /// <returns>An InvalidOperationException.</returns>
		public static InvalidOperationException ViewDataDictionary_ModelCannotBeNull(Type modelType)
		{
			string message = String.Format(CultureInfo.CurrentCulture, MvcResources.ViewDataDictionary_ModelCannotBeNull,
				modelType);
			return new InvalidOperationException(message);
		}

        /// <summary>Parameter cannot be null or empty.</summary>
        ///
        /// <param name="parameterName">Name of the parameter.</param>
        ///
        /// <returns>An ArgumentException.</returns>
        public static ArgumentException ParameterCannotBeNullOrEmpty(string parameterName)
        {
            return new ArgumentException(MvcResources.Common_NullOrEmpty, parameterName);
        }
	}
}
