using System.IO;

namespace NServiceKit.ServiceHost
{
    /// <summary>
    /// A thin wrapper around ASP.NET or HttpListener's HttpResponse
    /// </summary>
    public interface IHttpResponse
    {
        /// <summary>
        /// The underlying ASP.NET or HttpListener HttpResponse
        /// </summary>
        object OriginalResponse { get; }

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
        int StatusCode { get; set; }

        /// <summary>Gets or sets information describing the status.</summary>
        ///
        /// <value>Information describing the status.</value>
        string StatusDescription { get; set; }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        string ContentType { get; set; }

        /// <summary>Gets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
        ICookies Cookies { get; }

        /// <summary>Adds a header to 'value'.</summary>
        ///
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        void AddHeader(string name, string value);

        /// <summary>Redirects the given document.</summary>
        ///
        /// <param name="url">URL of the document.</param>
        void Redirect(string url);

        /// <summary>Gets the output stream.</summary>
        ///
        /// <value>The output stream.</value>
        Stream OutputStream { get; }

        /// <summary>Writes.</summary>
        ///
        /// <param name="text">The text to write.</param>
        void Write(string text);

        /// <summary>
        /// Signal that this response has been handled and no more processing should be done.
        /// When used in a request or response filter, no more filters or processing is done on this request.
        /// </summary>
        void Close();

        /// <summary>
        /// Calls Response.End() on ASP.NET HttpResponse otherwise is an alias for Close().
        /// Useful when you want to prevent ASP.NET to provide it's own custom error page.
        /// </summary>
        void End();

        /// <summary>
        /// Response.Flush() and OutputStream.Flush() seem to have different behaviour in ASP.NET
        /// </summary>
        void Flush();

        /// <summary>
        /// Gets a value indicating whether this instance is closed.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>Sets content length.</summary>
        ///
        /// <param name="contentLength">Length of the content.</param>
        void SetContentLength(long contentLength);
    }
}