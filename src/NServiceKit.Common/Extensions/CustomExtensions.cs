using System;

namespace NServiceKit.Common.Extensions
{
    /// <summary>A custom extensions.</summary>
	public static class CustomExtensions
	{
        /// <summary>An IConvertible extension method that toes the given value.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        ///
        /// <returns>A T.</returns>
		public static T To<T>(this IConvertible value)
		{
			try
			{
				return (T) Convert.ChangeType(value, typeof (T));
			}
			catch
			{
				return default(T);
			}
		}
	}
}


