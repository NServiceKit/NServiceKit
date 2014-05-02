using System;
using System.Web;
using NServiceKit.Logging;

namespace NServiceKit.WebHost.Endpoints.Support
{
    /// <summary>A HTTP handler base.</summary>
	public abstract class HttpHandlerBase : IHttpHandler
	{
		private readonly ILog log;

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.HttpHandlerBase class.</summary>
		protected HttpHandlerBase()
		{
			this.log = LogManager.GetLogger(this.GetType());
		}

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
		public void ProcessRequest(HttpContext context)
		{
			var before = DateTime.UtcNow;
			Execute(context);
            var elapsed = DateTime.UtcNow - before;
			log.DebugFormat("'{0}' was completed in {1}ms", this.GetType().Name, elapsed.TotalMilliseconds);
		}

        /// <summary>Executes the given context.</summary>
        ///
        /// <param name="context">The context.</param>
		public abstract void Execute(HttpContext context);

        /// <summary>Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.</summary>
        ///
        /// <value>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</value>
		public bool IsReusable
		{
			get { return false; }
		}
	}
}