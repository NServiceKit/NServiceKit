using System.IO;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.ServiceInterface.Testing
{
    /// <summary>A mock HTTP response.</summary>
    public class MockHttpResponse : IHttpResponse
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.MockHttpResponse class.</summary>
        public MockHttpResponse()
        {
            this.Cookies = new Cookies(this);
            this.OutputStream = new MemoryStream();
        }

        /// <summary>The underlying ASP.NET or HttpListener HttpResponse.</summary>
        ///
        /// <value>The original response.</value>
        public object OriginalResponse { get; private set; }

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
        }

        /// <summary>Redirects the given document.</summary>
        ///
        /// <param name="url">URL of the document.</param>
        public void Redirect(string url)
        {
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
        public void Flush()
        {
            this.OutputStream.Position = 0;
        }

        /// <summary>Reads as string.</summary>
        ///
        /// <returns>as string.</returns>
        public string ReadAsString()
        {
            var bytes = ((MemoryStream)OutputStream).ToArray();
            return bytes.FromUtf8Bytes();
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