using System;
using System.Collections.Generic;
using NServiceKit.Razor;
using NServiceKit.Razor.Managers;
using NServiceKit.VirtualPath;

namespace NServiceKit.ServiceHost.Tests.Formats_Razor
{
    /// <summary>A razor format extensions.</summary>
    public static class RazorFormatExtensions
    {
        /// <summary>A RazorFormat extension method that adds a file and page.</summary>
        ///
        /// <param name="razorFormat">The razorFormat to act on.</param>
        /// <param name="filePath">   Full pathname of the file.</param>
        /// <param name="contents">   The contents.</param>
        ///
        /// <returns>A RazorPage.</returns>
        public static RazorPage AddFileAndPage(this RazorFormat razorFormat, string filePath, string contents)
        {
            var pathProvider = (IWriteableVirtualPathProvider)razorFormat.VirtualPathProvider;
            pathProvider.AddFile(filePath, contents);
            return razorFormat.AddPage(filePath);
        }
    }
    
    /// <summary>A razor test base.</summary>
    public class RazorTestBase
	{
        /// <summary>Name of the template.</summary>
		public const string TemplateName = "Template";
        /// <summary>Name of the page.</summary>
		protected const string PageName = "Page";
        /// <summary>The razor format.</summary>
        protected RazorFormat RazorFormat;
	}

}
