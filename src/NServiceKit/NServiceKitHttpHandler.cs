using System.Web;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit
{
    /// <summary>A service kit HTTP handler.</summary>
    public class NServiceKitHttpHandler : IHttpHandler, INServiceKitHttpHandler
    {
        INServiceKitHttpHandler NServiceKitHandler;

        /// <summary>Initializes a new instance of the NServiceKit.NServiceKitHttpHandler class.</summary>
        ///
        /// <param name="NServiceKitHandler">The service kit handler.</param>
        public NServiceKitHttpHandler(INServiceKitHttpHandler NServiceKitHandler)
        {
            this.NServiceKitHandler = NServiceKitHandler;
        }

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
        public virtual void ProcessRequest(HttpContext context)
        {
            ProcessRequest(
                context.Request.ToRequest(), 
                context.Response.ToResponse(),
                null);
        }

        /// <summary>Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.</summary>
        ///
        /// <value>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</value>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>Process the request.</summary>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        public virtual void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            NServiceKitHandler.ProcessRequest(httpReq, httpRes, operationName ?? httpReq.OperationName);
        }
    }
}