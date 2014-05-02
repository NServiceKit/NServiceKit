using System;
using System.IO;
using System.Text;
using System.Web;
using NServiceKit.Common.Utils;

namespace NServiceKit.WebHost.Endpoints.Ext
{
	/// <summary>
	/// Summary description for $codebehindclassname$
	/// </summary>
	public class AllFilesHandler 
		: IHttpHandler
	{
        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
		public void ProcessRequest(HttpContext context)
		{
			var path = context.Request["Path"];
			if (string.IsNullOrEmpty(path))
			{
				path = context.Request.AppRelativeCurrentExecutionFilePath;
				if (!string.IsNullOrEmpty(path))
				{
					path = path.Substring(0, path.LastIndexOf("/"));
				}
				else
				{
					throw new ArgumentNullException("Path");
				}
			}

			context.Response.ContentType = context.Request["ContentType"] ?? "text/plain";
			var jsText = GetAllTextFiles(path, context.Request["Filter"] ?? "*.*");
			var wrapJs = context.Request["wrapJs"] != null;
			if (wrapJs)
			{
				jsText = WrapJavascriptInNamespace(jsText);
			}

			context.Response.Write(jsText);
		}

        /// <summary>Wrap javascript in namespace.</summary>
        ///
        /// <param name="jsText">The js text.</param>
        ///
        /// <returns>A string.</returns>
		public static string WrapJavascriptInNamespace(string jsText)
		{
			var sb = new StringBuilder();

			sb.Append("(function(){");
			sb.Append(jsText);
			sb.Append("})();");

			return sb.ToString();
		}

        /// <summary>In hidden directory.</summary>
        ///
        /// <param name="filePath">Full pathname of the file.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public static bool InHiddenDirectory(string filePath)
		{
			return filePath.Contains(".svn");
		}

        /// <summary>Gets all text files.</summary>
        ///
        /// <exception cref="UnauthorizedAccessException">Thrown when an Unauthorized Access error condition occurs.</exception>
        ///
        /// <param name="path">  Full pathname of the file.</param>
        /// <param name="filter">Specifies the filter.</param>
        ///
        /// <returns>all text files.</returns>
		public static string GetAllTextFiles(string path, string filter)
		{
			if (path.Contains(".."))
				throw new UnauthorizedAccessException("Invalid Path");

			var sb = new StringBuilder();

			var absolutePath = path.MapHostAbsolutePath();
			if (!Directory.Exists(absolutePath)) return null;

			foreach (var filePath in Directory.GetFiles(
				absolutePath, filter, SearchOption.AllDirectories))
			{
				if (InHiddenDirectory(filePath)) continue;

				//Can be optimized
				var fileText = File.ReadAllText(filePath);
				sb.AppendLine(fileText);
			}

			return sb.ToString();
		}

        /// <summary>Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.</summary>
        ///
        /// <value>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</value>
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}