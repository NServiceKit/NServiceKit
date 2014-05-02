using System;
using System.Collections.Generic;

namespace NServiceKit.Common.Utils
{
    /// <summary>An assert utilities.</summary>
    public static class AssertUtils
    {
        /// <summary>Are not null.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="fields">A variable-length parameters list containing fields.</param>
        public static void AreNotNull<T>(params T[] fields) where T : class 
        {
            foreach (var field in fields)
            {
                if (field == null)
                {
                    throw new ArgumentNullException(typeof(T).Name);
                }
            }
        }

        /// <summary>
        /// Asserts that the supplied arguments are not null.
        /// 
        /// AssertUtils.AreNotNull(new Dictionary&lt;string,object&gt;{ {"name",null} });
        ///   will throw new ArgumentNullException("name");
        /// </summary>
        /// <param name="fieldMap">The field map.</param>
        public static void AreNotNull(IDictionary<string,object> fieldMap)
        {
            foreach (var pair in fieldMap)
            {
                if (pair.Value == null)
                {
                    throw new ArgumentNullException(pair.Key);
                }
            }
        }
    }
}