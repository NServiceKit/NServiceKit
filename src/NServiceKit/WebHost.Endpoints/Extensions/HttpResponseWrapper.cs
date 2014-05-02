using System;
using System.Globalization;
using System.IO;
using System.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Extensions
{
    /// <summary>A HTTP response wrapper.</summary>
    public class HttpResponseWrapper
        : IHttpResponse
    {
        //private static readonly ILog Log = LogManager.GetLogger(typeof(HttpResponseWrapper));

        private readonly HttpResponse response;

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Extensions.HttpResponseWrapper class.</summary>
        ///
        /// <param name="response">The response.</param>
        public HttpResponseWrapper(HttpResponse response)
        {
            this.response = response;
            this.response.TrySkipIisCustomErrors = true;
            this.Cookies = new Cookies(this);
        }

        /// <summary>Gets the response.</summary>
        ///
        /// <value>The response.</value>
        public HttpResponse Response
        {
            get { return response; }
        }

        /// <summary>The underlying ASP.NET or HttpListener HttpResponse.</summary>
        ///
        /// <value>The original response.</value>
        public object OriginalResponse
        {
            get { return response; }
        }

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
        public int StatusCode
        {
            get { return this.response.StatusCode; }
            set { this.response.StatusCode = value; }
        }

        /// <summary>Gets or sets information describing the status.</summary>
        ///
        /// <value>Information describing the status.</value>
        public string StatusDescription
        {
            get { return this.response.StatusDescription; }
            set { this.response.StatusDescription = value; }
        }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get { return response.ContentType; }
            set { response.ContentType = value; }
        }

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
            response.AddHeader(name, value);
        }

        /// <summary>Redirects the given document.</summary>
        ///
        /// <param name="url">URL of the document.</param>
        public void Redirect(string url)
        {
            response.Redirect(url);
        }

        /// <summary>Gets the output stream.</summary>
        ///
        /// <value>The output stream.</value>
        public Stream OutputStream
        {
            get { return response.OutputStream; }
        }

        /// <summary>Writes.</summary>
        ///
        /// <param name="text">The text to write.</param>
        public void Write(string text)
        {
            response.Write(text);
        }

        /// <summary>
        /// Signal that this response has been handled and no more processing should be done. When used in a request or response filter, no more filters or processing is done on this request.
        /// </summary>
        public void Close()
        {
            this.IsClosed = true;
            response.CloseOutputStream();
        }

        /// <summary>Calls Response.End() on ASP.NET HttpResponse otherwise is an alias for Close(). Useful when you want to prevent ASP.NET to provide it's own custom error page.</summary>
        public void End()
        {
            this.IsClosed = true;
            try
            {
                response.ClearContent();
                response.End();
            }
            catch { }
        }

        /// <summary>Response.Flush() and OutputStream.Flush() seem to have different behaviour in ASP.NET.</summary>
        public void Flush()
        {
            response.Flush();
        }

        /// <summary>Gets a value indicating whether this instance is closed.</summary>
        ///
        /// <value>true if this object is closed, false if not.</value>
        public bool IsClosed
        {
            get;
            private set;
        }

        /// <summary>Sets content length.</summary>
        ///
        /// <param name="contentLength">Length of the content.</param>
        public void SetContentLength(long contentLength)
        {
            try
            {
                response.Headers.Add("Content-Length", contentLength.ToString(CultureInfo.InvariantCulture));
            }
            catch (PlatformNotSupportedException /*ignore*/) { } //This operation requires IIS integrated pipeline mode.
        }
    }
}