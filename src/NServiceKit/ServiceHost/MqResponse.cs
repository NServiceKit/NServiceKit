using System;
using System.Collections.Generic;
using System.IO;
using NServiceKit.Text;

namespace NServiceKit.ServiceHost
{
    /// <summary>A mq response.</summary>
    public class MqResponse : IHttpResponse
    {
        private readonly MqRequestContext requestContext;
        private Dictionary<string, string> Headers { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.MqResponse class.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        public MqResponse(MqRequestContext requestContext)
        {
            this.requestContext = requestContext;
            this.Headers = new Dictionary<string, string>();
        }

        /// <summary>The underlying ASP.NET or HttpListener HttpResponse.</summary>
        ///
        /// <value>The original response.</value>
        public object OriginalResponse { get; set; }

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
        public int StatusCode { get; set; }

        /// <summary>Gets or sets information describing the status.</summary>
        ///
        /// <value>Information describing the status.</value>
        public string StatusDescription { get; set; }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get { return requestContext.ResponseContentType; }
            set { requestContext.ResponseContentType = value; }
        }

        /// <summary>Gets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
        public ICookies Cookies
        {
            get { return null; }
        }

        /// <summary>Adds a header to 'value'.</summary>
        ///
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        public void AddHeader(string name, string value)
        {
            Headers[name] = value;
        }

        /// <summary>Redirects the given document.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="url">URL of the document.</param>
        public void Redirect(string url)
        {
            throw new NotImplementedException();
        }

        private MemoryStream ms;

        /// <summary>Gets the output stream.</summary>
        ///
        /// <value>The output stream.</value>
        public Stream OutputStream
        {
            get { return ms ?? (ms = new MemoryStream()); }
        }

        /// <summary>Writes.</summary>
        ///
        /// <param name="text">The text to write.</param>
        public void Write(string text)
        {
            var bytes = text.ToUtf8Bytes();
            ms.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Signal that this response has been handled and no more processing should be done. When used in a request or response filter, no more filters or processing is done on this request.
        /// </summary>
        public void Close()
        {
            IsClosed = true;
        }

        /// <summary>Calls Response.End() on ASP.NET HttpResponse otherwise is an alias for Close(). Useful when you want to prevent ASP.NET to provide it's own custom error page.</summary>
        public void End()
        {
            Close();
        }

        /// <summary>Response.Flush() and OutputStream.Flush() seem to have different behaviour in ASP.NET.</summary>
        public void Flush() {}

        /// <summary>Gets or sets a value indicating whether this instance is closed.</summary>
        ///
        /// <value>true if this object is closed, false if not.</value>
        public bool IsClosed { get; set; }

        /// <summary>Sets content length.</summary>
        ///
        /// <param name="contentLength">Length of the content.</param>
        public void SetContentLength(long contentLength) {}
    }
}