using System;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.IO;

namespace NServiceKit.VirtualPath
{
    /// <summary>A virtual path extension.</summary>
    public static class VirtualPathExtension
    {
        /// <summary>A string extension method that tokenize virtual path.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="str">         The str to act on.</param>
        /// <param name="pathProvider">The path provider.</param>
        ///
        /// <returns>A Stack&lt;string&gt;</returns>
        public static Stack<string> TokenizeVirtualPath(this string str, IVirtualPathProvider pathProvider)
        {
            if (pathProvider == null)
                throw new ArgumentNullException("pathProvider");

            return TokenizeVirtualPath(str, pathProvider.VirtualPathSeparator);
        }

        /// <summary>A string extension method that tokenize virtual path.</summary>
        ///
        /// <param name="str">                 The str to act on.</param>
        /// <param name="virtualPathSeparator">The virtual path separator.</param>
        ///
        /// <returns>A Stack&lt;string&gt;</returns>
        public static Stack<string> TokenizeVirtualPath(this string str, string virtualPathSeparator)
        {
            if (string.IsNullOrEmpty(str))
                return new Stack<string>();

            var tokens = str.Split(new[] { virtualPathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            return new Stack<string>(tokens.Reverse());
        }

        /// <summary>A string extension method that tokenize resource path.</summary>
        ///
        /// <param name="str">          The str to act on.</param>
        /// <param name="pathSeparator">The path separator.</param>
        ///
        /// <returns>A Stack&lt;string&gt;</returns>
        public static Stack<string> TokenizeResourcePath(this string str, char pathSeparator = '.')
        {
            if (string.IsNullOrEmpty(str))
                return new Stack<string>();

            var n = str.Count(c => c == pathSeparator);
            var tokens = str.Split(new [] { pathSeparator }, n);

            return new Stack<string>(tokens.Reverse());
        }

        /// <summary>Enumerates group by first token in this collection.</summary>
        ///
        /// <param name="resourceNames">The resourceNames to act on.</param>
        /// <param name="pathSeparator">The path separator.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process group by first token in this collection.</returns>
        public static IEnumerable<IGrouping<string, string[]>> GroupByFirstToken(this IEnumerable<string> resourceNames, char pathSeparator = '.')
        {
            return resourceNames.Select(n => n.Split(new[] { pathSeparator }, 2))
                .GroupBy(t => t[0]);
        }
    }
}
