using System;
using System.Net;
using System.Web;
using NServiceKit.Logging;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Extensions
{
    /// <summary>A HTTP response stream extensions.</summary>
	public static class HttpResponseStreamExtensions
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(HttpResponseStreamExtensions));
		//public static bool IsXsp;
		//public static bool IsModMono;
        /// <summary>true if this object is mono fast CGI.</summary>
		public static bool IsMonoFastCgi;
		//public static bool IsWebDevServer;
		//public static bool IsIis;
        /// <summary>true if this object is HTTP listener.</summary>
		public static bool IsHttpListener;

		static HttpResponseStreamExtensions()
		{
			//IsXsp = Env.IsMono;
			//IsModMono = Env.IsMono;
			IsMonoFastCgi = Env.IsMono;

			//IsWebDevServer = !Env.IsMono;
			//IsIis = !Env.IsMono;
			IsHttpListener = HttpContext.Current == null;
		}

        /// <summary>A HttpListenerResponse extension method that closes output stream.</summary>
        ///
        /// <param name="response">The response to act on.</param>
		public static void CloseOutputStream(this HttpResponse response)
		{
			try
			{
				//Don't close for MonoFastCGI as it outputs random 4-letters at the start
				if (!IsMonoFastCgi)
				{
					response.OutputStream.Flush();
					response.OutputStream.Close();
					//response.Close(); //This kills .NET Development Web Server
				}
			}
			catch (Exception ex)
			{
				Log.Error("Exception closing HttpResponse: " + ex.Message, ex);
			}
		}

        /// <summary>A HttpListenerResponse extension method that closes output stream.</summary>
        ///
        /// <param name="response">The response to act on.</param>
		public static void CloseOutputStream(this HttpListenerResponse response)
		{
			try
			{
				response.OutputStream.Flush();
				response.OutputStream.Close();
				response.Close();
			}
			catch (Exception ex)
			{
				Log.Error("Error in HttpListenerResponseWrapper: " + ex.Message, ex);
			}
		}

	}
}