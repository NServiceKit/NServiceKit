using System.Collections.Generic;
using System.IO;
using System.Text;
using NServiceKit.ServiceHost;
using NServiceKit.Common;

namespace NServiceKit.Common.Web
{
    /// <summary>A HTTP response stream wrapper.</summary>
    public class HttpResponseStreamWrapper : IHttpResponse
    {
        private static readonly UTF8Encoding UTF8EncodingWithoutBom = new UTF8Encoding(false);

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResponseStreamWrapper class.</summary>
        ///
        /// <param name="stream">The stream.</param>
        public HttpResponseStreamWrapper(Stream stream)
        {
            this.OutputStream = stream;
            this.Headers = new Dictionary<string, string>();
        }

        /// <summary>Gets or sets the headers.</summary>
        ///
        /// <value>The headers.</value>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>The underlying ASP.NET or HttpListener HttpResponse.</summary>
        ///
        /// <value>The original response.</value>
        public object OriginalResponse
        {
            get { return null; }
        }

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
        public int StatusCode { set; get; }

        /// <summary>Gets or sets information describing the status.</summary>
        ///
        /// <value>Information describing the status.</value>
        public string StatusDescription { set; get; }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>Gets or sets a value indicating whether the keep open.</summary>
        ///
        /// <value>true if keep open, false if not.</value>
        public bool KeepOpen { get; set; }

        /// <summary>Gets or sets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
        public ICookies Cookies { get; set; }

        /// <summary>Adds a header to 'value'.</summary>
        ///
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        public void AddHeader(string name, string value)
        {
            this.Headers[name] = value;
        }

        /// <summary>Redirects the given document.</summary>
        ///
        /// <param name="url">URL of the document.</param>
        public void Redirect(string url)
        {
            this.Headers[HttpHeaders.Location] = url;
        }

        /// <summary>Gets the output stream.</summary>
        ///
        /// <value>The output stream.</value>
        public Stream OutputStream { get; private set; }

        /// <summary>Writes.</summary>
        ///
        /// <param name="text">The text to write.</param>
        public void Write(string text)
        {
            var bytes = UTF8EncodingWithoutBom.GetBytes(text);
            OutputStream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Signal that this response has been handled and no more processing should be done. When used in a request or response filter, no more filters or processing is done on this request.
        /// </summary>
        public void Close()
        {
            if (KeepOpen) return;
            ForceClose();
        }

        /// <summary>Force close.</summary>
        public void ForceClose()
        {
            if (IsClosed) return;

            OutputStream.Close();
            IsClosed = true;
        }

        /// <summary>Calls Response.End() on ASP.NET HttpResponse otherwise is an alias for Close(). Useful when you want to prevent ASP.NET to provide it's own custom error page.</summary>
        public void End()
        {
            Close();
        }

        /// <summary>Response.Flush() and OutputStream.Flush() seem to have different behaviour in ASP.NET.</summary>
        public void Flush()
        {
            OutputStream.Flush();
        }

        /// <summary>Gets a value indicating whether this instance is closed.</summary>
        ///
        /// <value>true if this object is closed, false if not.</value>
        public bool IsClosed { get; private set; }

        /// <summary>Sets content length.</summary>
        ///
        /// <param name="contentLength">Length of the content.</param>
        public void SetContentLength(long contentLength) {}
    }
}