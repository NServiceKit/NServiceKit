using System;
using System.IO;
using System.Web;
using NServiceKit.Common.Utils;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.ServiceHost
{
    /// <summary>A file extensions.</summary>
	public static class FileExtensions
	{
        /// <summary>An IFile extension method that saves to.</summary>
        ///
        /// <param name="file">    The file to act on.</param>
        /// <param name="filePath">Full pathname of the file.</param>
		public static void SaveTo(this IFile file, string filePath)
		{
			using (var sw = new StreamWriter(filePath, false))
			{
				file.InputStream.WriteTo(sw.BaseStream);
			}
		}

        /// <summary>An IFile extension method that writes to.</summary>
        ///
        /// <param name="file">  The file to act on.</param>
        /// <param name="stream">The stream.</param>
		public static void WriteTo(this IFile file, Stream stream)
		{
			file.InputStream.WriteTo(stream);
		}

        /// <summary>A string extension method that map server path.</summary>
        ///
        /// <param name="relativePath">The relativePath to act on.</param>
        ///
        /// <returns>A string.</returns>
		public static string MapServerPath(this string relativePath)
		{
			var isAspNetHost = HttpListenerBase.Instance == null || HttpContext.Current != null;
			var appHost = EndpointHost.AppHost;
			if (appHost != null)
			{
				isAspNetHost = !(appHost is HttpListenerBase);
			}

			return isAspNetHost
			       ? relativePath.MapHostAbsolutePath()
			       : relativePath.MapAbsolutePath();
		}

        /// <summary>A string extension method that query if 'relativeOrAbsolutePath' is relative path.</summary>
        ///
        /// <param name="relativeOrAbsolutePath">The relativeOrAbsolutePath to act on.</param>
        ///
        /// <returns>true if relative path, false if not.</returns>
		public static bool IsRelativePath(this string relativeOrAbsolutePath)
		{
			return !relativeOrAbsolutePath.Contains(":")
				&& !relativeOrAbsolutePath.StartsWith("/") 
				&& !relativeOrAbsolutePath.StartsWith("\\");
		}

        /// <summary>A FileInfo extension method that reads a fully.</summary>
        ///
        /// <param name="file">The file to act on.</param>
        ///
        /// <returns>An array of byte.</returns>
        public static byte[] ReadFully(this FileInfo file)
        {
            using (var fs = file.OpenRead())
            {
                return fs.ReadFully();
            }
        }
	}
}