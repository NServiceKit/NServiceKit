using System.Collections.Generic;
using System.Web;
using NServiceKit.Common;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.WebHost.Endpoints.Support
{
    /// <summary>A forbidden HTTP handler.</summary>
    public class ForbiddenHttpHandler
        : INServiceKitHttpHandler, IHttpHandler
    {
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

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="request">      The request.</param>
        /// <param name="response">     The response.</param>
        /// <param name="operationName">Name of the operation.</param>
        public void ProcessRequest(IHttpRequest request, IHttpResponse response, string operationName)
        {
            response.ContentType = "text/plain";
            response.StatusCode = 403;

		    response.EndHttpHandlerRequest(skipClose: true, afterBody: r => {
                r.Write("Forbidden\n\n");

                r.Write("\nRequest.HttpMethod: " + request.HttpMethod);
                r.Write("\nRequest.PathInfo: " + request.PathInfo);
                r.Write("\nRequest.QueryString: " + request.QueryString);
                r.Write("\nRequest.RawUrl: " + request.RawUrl);

                if (IsIntegratedPipeline.HasValue)
                    r.Write("\nApp.IsIntegratedPipeline: " + IsIntegratedPipeline);
                if (!WebHostPhysicalPath.IsNullOrEmpty())
                    r.Write("\nApp.WebHostPhysicalPath: " + WebHostPhysicalPath);
                if (!WebHostRootFileNames.IsEmpty())
                    r.Write("\nApp.WebHostRootFileNames: " + TypeSerializer.SerializeToString(WebHostRootFileNames));
                if (!ApplicationBaseUrl.IsNullOrEmpty())
                    r.Write("\nApp.ApplicationBaseUrl: " + ApplicationBaseUrl);
                if (!DefaultRootFileName.IsNullOrEmpty())
                    r.Write("\nApp.DefaultRootFileName: " + DefaultRootFileName);
                if (!DefaultHandler.IsNullOrEmpty())
                    r.Write("\nApp.DefaultHandler: " + DefaultHandler);
                if (!NServiceKitHttpHandlerFactory.DebugLastHandlerArgs.IsNullOrEmpty())
                    r.Write("\nApp.DebugLastHandlerArgs: " + NServiceKitHttpHandlerFactory.DebugLastHandlerArgs);
            });
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

            response.ContentType = "text/plain";
            response.StatusCode = 403;

            response.EndHttpHandlerRequest(skipClose:true, afterBody: r=> {
                r.Write("Forbidden\n\n");

                r.Write("\nRequest.HttpMethod: " + request.HttpMethod);
                r.Write("\nRequest.PathInfo: " + request.PathInfo);
                r.Write("\nRequest.QueryString: " + request.QueryString);
                r.Write("\nRequest.RawUrl: " + request.RawUrl);

                if (IsIntegratedPipeline.HasValue)
                    r.Write("\nApp.IsIntegratedPipeline: " + IsIntegratedPipeline);
                if (!WebHostPhysicalPath.IsNullOrEmpty())
                    r.Write("\nApp.WebHostPhysicalPath: " + WebHostPhysicalPath);
                if (!WebHostRootFileNames.IsEmpty())
                    r.Write("\nApp.WebHostRootFileNames: " + TypeSerializer.SerializeToString(WebHostRootFileNames));
                if (!ApplicationBaseUrl.IsNullOrEmpty())
                    r.Write("\nApp.ApplicationBaseUrl: " + ApplicationBaseUrl);
                if (!DefaultRootFileName.IsNullOrEmpty())
                    r.Write("\nApp.DefaultRootFileName: " + DefaultRootFileName);
            });
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