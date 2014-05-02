using System;
using System.Collections.Generic;

namespace NServiceKit.ServiceInterface.ServiceModel
{
	/// <summary>
	/// A log entry added by the IRequestLogger
	/// </summary>
	public class RequestLogEntry
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public long Id { get; set; }

        /// <summary>Gets or sets the date time.</summary>
        ///
        /// <value>The date time.</value>
		public DateTime DateTime { get; set; }

        /// <summary>Gets or sets the HTTP method.</summary>
        ///
        /// <value>The HTTP method.</value>
		public string HttpMethod { get; set; }

        /// <summary>Gets or sets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
		public string AbsoluteUri { get; set; }

        /// <summary>Gets or sets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
		public string PathInfo { get; set; }

        /// <summary>Gets or sets the request body.</summary>
        ///
        /// <value>The request body.</value>
        public string RequestBody { get; set; }

        /// <summary>Gets or sets the request dto.</summary>
        ///
        /// <value>The request dto.</value>
        public object RequestDto { get; set; }

        /// <summary>Gets or sets the identifier of the user authentication.</summary>
        ///
        /// <value>The identifier of the user authentication.</value>
        public string UserAuthId { get; set; }

        /// <summary>Gets or sets the identifier of the session.</summary>
        ///
        /// <value>The identifier of the session.</value>
		public string SessionId { get; set; }

        /// <summary>Gets or sets the IP address.</summary>
        ///
        /// <value>The IP address.</value>
		public string IpAddress { get; set; }

        /// <summary>Gets or sets the forwarded for.</summary>
        ///
        /// <value>The forwarded for.</value>
		public string ForwardedFor { get; set; }

        /// <summary>Gets or sets the referer.</summary>
        ///
        /// <value>The referer.</value>
		public string Referer { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        ///
        /// <value>The headers.</value>
		public Dictionary<string, string> Headers { get; set; }

        /// <summary>Gets or sets information describing the form.</summary>
        ///
        /// <value>Information describing the form.</value>
		public Dictionary<string, string> FormData { get; set; }

        /// <summary>Gets or sets the items.</summary>
        ///
        /// <value>The items.</value>
		public Dictionary<string, object> Items { get; set; }

        /// <summary>Gets or sets the session.</summary>
        ///
        /// <value>The session.</value>
		public object Session { get; set; }

        /// <summary>Gets or sets the response dto.</summary>
        ///
        /// <value>The response dto.</value>
		public object ResponseDto { get; set; }

        /// <summary>Gets or sets the error response.</summary>
        ///
        /// <value>The error response.</value>
		public object ErrorResponse { get; set; }

        /// <summary>Gets or sets the duration of the request.</summary>
        ///
        /// <value>The request duration.</value>
        public TimeSpan RequestDuration { get; set; }
	}
}