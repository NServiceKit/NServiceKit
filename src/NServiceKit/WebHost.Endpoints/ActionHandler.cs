using System;
using System.Web;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>An action handler.</summary>
    public class ActionHandler : INServiceKitHttpHandler, IHttpHandler 
    {
        /// <summary>Gets or sets the name of the operation.</summary>
        ///
        /// <value>The name of the operation.</value>
        public string OperationName { get; set; }

        /// <summary>Gets or sets the action.</summary>
        ///
        /// <value>The action.</value>
        public Func<IHttpRequest, IHttpResponse, object> Action { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.ActionHandler class.</summary>
        ///
        /// <param name="action">       The action.</param>
        /// <param name="operationName">Name of the operation.</param>
        public ActionHandler(Func<IHttpRequest, IHttpResponse, object> action, string operationName=null)
        {
            Action = action;
            OperationName = operationName;
        }

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        public void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            if (Action == null)
                throw new Exception("Action was not supplied to ActionHandler");
            
            if (httpReq.OperationName == null)
                httpReq.SetOperationName(OperationName);

            var response = Action(httpReq, httpRes);
            httpRes.WriteToResponse(httpReq, response);
        }

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(context.Request.ToRequest(OperationName), 
                context.Response.ToResponse(),
                OperationName);
        }

        /// <summary>Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.</summary>
        ///
        /// <value>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</value>
        public bool IsReusable
        {
            get { return false; }
        }
    }
}