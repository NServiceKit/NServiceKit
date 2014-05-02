using System;
using System.Web;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Web;

namespace NServiceKit.ServiceHost
{
    /// <summary>A request attributes.</summary>
	public class RequestAttributes : IRequestAttributes
	{
		private readonly HttpContext httpContext;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.RequestAttributes class.</summary>
        ///
        /// <param name="httpRequest">The HTTP request.</param>
		public RequestAttributes(IHttpRequest httpRequest)
		{
			this.acceptEncoding = httpRequest.Headers[HttpHeaders.AcceptEncoding];
			if (this.acceptEncoding.IsNullOrEmpty())
			{
				this.acceptEncoding = "none";
				return;
			}
			this.acceptEncoding = this.acceptEncoding.ToLower();
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.RequestAttributes class.</summary>
        ///
        /// <param name="httpContext">Context for the HTTP.</param>
		public RequestAttributes(HttpContext httpContext)
		{
			this.httpContext = httpContext;
		}

        /// <summary>Gets a worker.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>The worker.</returns>
		public static HttpWorkerRequest GetWorker(HttpContext context)
		{
			var provider = (IServiceProvider)context;
			var worker = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
			return worker;
		}

		private HttpWorkerRequest httpWorkerRequest;

        /// <summary>Gets the HTTP worker request.</summary>
        ///
        /// <value>The HTTP worker request.</value>
		public HttpWorkerRequest HttpWorkerRequest
		{
			get
			{
				if (this.httpWorkerRequest == null)
				{
					this.httpWorkerRequest = GetWorker(this.httpContext);
				}
				return this.httpWorkerRequest;
			}
		}

		private string acceptEncoding;

        /// <summary>Gets the accept encoding.</summary>
        ///
        /// <value>The accept encoding.</value>
		public string AcceptEncoding
		{
			get
			{
				if (acceptEncoding == null)
				{
					acceptEncoding = HttpWorkerRequest.GetKnownRequestHeader(HttpWorkerRequest.HeaderAcceptEncoding);
					if (acceptEncoding != null) acceptEncoding = acceptEncoding.ToLower();
				}
				return acceptEncoding;
			}
		}

        /// <summary>Gets a value indicating whether the accepts gzip.</summary>
        ///
        /// <value>true if accepts gzip, false if not.</value>
		public bool AcceptsGzip
		{
			get
			{
				return AcceptEncoding != null && AcceptEncoding.Contains("gzip");
			}
		}

        /// <summary>Gets a value indicating whether the accepts deflate.</summary>
        ///
        /// <value>true if accepts deflate, false if not.</value>
		public bool AcceptsDeflate
		{
			get
			{
				return AcceptEncoding != null && AcceptEncoding.Contains("deflate");
			}
		}
	
	}
}