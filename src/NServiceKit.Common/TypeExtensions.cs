using System;
using System.Collections.Generic;
using NServiceKit.Common;
using NServiceKit.Text;

namespace NServiceKit.Common
{
    /// <summary>A type extensions.</summary>
    public static class TypeExtensions
    {
        private static readonly Dictionary<Type, List<string>> TypePropertyNamesMap = new Dictionary<Type, List<string>>();

        /// <summary>A Type extension method that gets property names.</summary>
        ///
        /// <param name="type">The type to act on.</param>
        ///
        /// <returns>The property names.</returns>
        public static List<string> GetPropertyNames(this Type type)
        {
            lock (TypePropertyNamesMap)
            {
                List<string> propertyNames;
                if (!TypePropertyNamesMap.TryGetValue(type, out propertyNames))
                {
                    propertyNames = type.Properties().SafeConvertAll(x => x.Name);
                    TypePropertyNamesMap[type] = propertyNames;
                }
                return propertyNames;
            }
        }

        /// <summary>A Type extension method that converts a type to the attributes.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="type">The type to act on.</param>
        ///
        /// <returns>type as a List&lt;T&gt;</returns>
        public static List<T> ToAttributes<T>(this Type type) where T : Attribute
        {
            return type.CustomAttributes(typeof(T)).SafeConvertAll(x => (T)x);
        }

#if !SILVERLIGHT

        /// <summary>A Type extension method that gets assembly path.</summary>
        ///
        /// <param name="source">The source to act on.</param>
        ///
        /// <returns>The assembly path.</returns>
        public static string GetAssemblyPath(this Type source)
        {
            var assemblyUri =
                new Uri(source.Assembly.EscapedCodeBase);

            return assemblyUri.LocalPath;
        }
#endif
    }
}
