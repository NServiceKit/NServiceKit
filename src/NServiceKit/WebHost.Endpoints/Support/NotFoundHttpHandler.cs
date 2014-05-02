using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using NServiceKit.Common;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.Logging;
using HttpRequestWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpRequestWrapper;
using HttpResponseWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpResponseWrapper;

namespace NServiceKit.WebHost.Endpoints.Support
{
    /// <summary>A not found HTTP handler.</summary>
	public class NotFoundHttpHandler
		: INServiceKitHttpHandler, IHttpHandler
	{
        private static readonly ILog Log = LogManager.GetLogger(typeof(NotFoundHttpHandler));

        /// <summary>Gets or sets the is integrated pipeline.</summary>
        ///
        /// <value>The is integrated pipeline.</value>
		public bool? IsIntegratedPipeline { get; set; }

        /// <summary>Gets or sets the full pathname of the web host physical file.</summary>
        ///
        /// <value>The full pathname of the web host physical file.</value>
		public string WebHostPhysicalPath { get; set; }

        /// <summary>Gets or sets a list of names of the web host root files.</summary>
        ///
        /// <value>A list of names of the web host root files.</value>
		public List<string> WebHostRootFileNames { get; set; }

        /// <summary>Gets or sets URL of the application base.</summary>
        ///
        /// <value>The application base URL.</value>
		public string ApplicationBaseUrl { get; set; }

        /// <summary>Gets or sets the default root file name.</summary>
        ///
        /// <value>The default root file name.</value>
		public string DefaultRootFileName { get; set; }

        /// <summary>Gets or sets the default handler.</summary>
        ///
        /// <value>The default handler.</value>
		public string DefaultHandler { get; set; }

        /// <summary>Process the request.</summary>
        ///
        /// <param name="request">      The HTTP request.</param>
        /// <param name="response">     The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
		public void ProcessRequest(IHttpRequest request, IHttpResponse response, string operationName)
		{
            Log.ErrorFormat("{0} Request not found: {1}", request.UserHostAddress, request.RawUrl);

		    var text = new StringBuilder();

            if (EndpointHost.DebugMode)
            {
                text.AppendLine("Handler for Request not found: \n\n")
                    .AppendLine("Request.HttpMethod: " + request.HttpMethod)
                    .AppendLine("Request.HttpMethod: " + request.HttpMethod)
                    .AppendLine("Request.PathInfo: " + request.PathInfo)
                    .AppendLine("Request.QueryString: " + request.QueryString)
                    .AppendLine("Request.RawUrl: " + request.RawUrl);
            }
            else
            {
                text.Append("404");
            }

		    response.ContentType = "text/plain";
			response.StatusCode = 404;
            response.EndHttpHandlerRequest(skipClose: true, afterBody: r => r.Write(text.ToString()));
		}

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
		public void ProcessRequest(HttpContext context)
		{
			var request = context.Request;
			var response = context.Response;

			var httpReq = new HttpRequestWrapper("NotFoundHttpHandler", request);
			if (!request.IsLocal)
			{
				ProcessRequest(httpReq, new HttpResponseWrapper(response), null);
				return;
			}

            Log.ErrorFormat("{0} Request not found: {1}", request.UserHostAddress, request.RawUrl);

			var sb = new StringBuilder();
			sb.AppendLine("Handler for Request not found: \n\n");

			sb.AppendLine("Request.ApplicationPath: " + request.ApplicationPath);
			sb.AppendLine("Request.CurrentExecutionFilePath: " + request.CurrentExecutionFilePath);
			sb.AppendLine("Request.FilePath: " + request.FilePath);
			sb.AppendLine("Request.HttpMethod: " + request.HttpMethod);
			sb.AppendLine("Request.MapPath('~'): " + request.MapPath("~"));
			sb.AppendLine("Request.Path: " + request.Path);
			sb.AppendLine("Request.PathInfo: " + request.PathInfo);
			sb.AppendLine("Request.ResolvedPathInfo: " + httpReq.PathInfo);
			sb.AppendLine("Request.PhysicalPath: " + request.PhysicalPath);
			sb.AppendLine("Request.PhysicalApplicationPath: " + request.PhysicalApplicationPath);
			sb.AppendLine("Request.QueryString: " + request.QueryString);
			sb.AppendLine("Request.RawUrl: " + request.RawUrl);
			try
			{
				sb.AppendLine("Request.Url.AbsoluteUri: " + request.Url.AbsoluteUri);
				sb.AppendLine("Request.Url.AbsolutePath: " + request.Url.AbsolutePath);
				sb.AppendLine("Request.Url.Fragment: " + request.Url.Fragment);
				sb.AppendLine("Request.Url.Host: " + request.Url.Host);
				sb.AppendLine("Request.Url.LocalPath: " + request.Url.LocalPath);
				sb.AppendLine("Request.Url.Port: " + request.Url.Port);
				sb.AppendLine("Request.Url.Query: " + request.Url.Query);
				sb.AppendLine("Request.Url.Scheme: " + request.Url.Scheme);
				sb.AppendLine("Request.Url.Segments: " + request.Url.Segments);
			}
			catch (Exception ex)
			{
				sb.AppendLine("Request.Url ERROR: " + ex.Message);
			}
			if (IsIntegratedPipeline.HasValue)
				sb.AppendLine("App.IsIntegratedPipeline: " + IsIntegratedPipeline);
			if (!WebHostPhysicalPath.IsNullOrEmpty())
				sb.AppendLine("App.WebHostPhysicalPath: " + WebHostPhysicalPath);
			if (!WebHostRootFileNames.IsEmpty())
				sb.AppendLine("App.WebHostRootFileNames: " + TypeSerializer.SerializeToString(WebHostRootFileNames));
			if (!ApplicationBaseUrl.IsNullOrEmpty())
				sb.AppendLine("App.ApplicationBaseUrl: " + ApplicationBaseUrl);
			if (!DefaultRootFileName.IsNullOrEmpty())
				sb.AppendLine("App.DefaultRootFileName: " + DefaultRootFileName);
			if (!DefaultHandler.IsNullOrEmpty())
				sb.AppendLine("App.DefaultHandler: " + DefaultHandler);
			if (!NServiceKitHttpHandlerFactory.DebugLastHandlerArgs.IsNullOrEmpty())
				sb.AppendLine("App.DebugLastHandlerArgs: " + NServiceKitHttpHandlerFactory.DebugLastHandlerArgs);

			response.ContentType = "text/plain";
			response.StatusCode = 404;
            response.EndHttpHandlerRequest(skipClose:true, afterBody: r => r.Write(sb.ToString()));
		}

        /// <summary>Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.</summary>
        ///
        /// <value>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</value>
		public bool IsReusable
		{
			get { return true; }
		}
	}
}