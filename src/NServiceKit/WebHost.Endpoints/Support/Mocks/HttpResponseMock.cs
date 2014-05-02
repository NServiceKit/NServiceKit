using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Mocks
{
    /// <summary>A HTTP response mock.</summary>
	public class HttpResponseMock
		: IHttpResponse
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Mocks.HttpResponseMock class.</summary>
		public HttpResponseMock()
		{
			this.Headers = new NameValueCollection();
			this.OutputStream = new MemoryStream();
			this.TextWritten = new StringBuilder();
			this.Cookies = new Cookies(this);
		}

        /// <summary>The underlying ASP.NET or HttpListener HttpResponse.</summary>
        ///
        /// <value>The original response.</value>
		public object OriginalResponse
		{
			get { return null; }
		}

        /// <summary>Gets output stream as string.</summary>
        ///
        /// <returns>The output stream as string.</returns>
		public string GetOutputStreamAsString()
		{
			this.OutputStream.Seek(0, SeekOrigin.Begin);
			using (var reader = new StreamReader(this.OutputStream))
			{
				return reader.ReadToEnd();
			}
		}

        /// <summary>Gets output stream as bytes.</summary>
        ///
        /// <returns>An array of byte.</returns>
		public byte[] GetOutputStreamAsBytes()
		{
			var ms = (MemoryStream)this.OutputStream;
			return ms.ToArray();
		}

        /// <summary>Gets or sets the text written.</summary>
        ///
        /// <value>The text written.</value>
		public StringBuilder TextWritten
		{
			get;
			set;
		}

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
		public int StatusCode { get; set; }

		private string statusDescription = string.Empty;

        /// <summary>Gets or sets information describing the status.</summary>
        ///
        /// <value>Information describing the status.</value>
		public string StatusDescription
		{
			get
			{
				return statusDescription;
			}
			set
			{
				statusDescription = value;
			}
		}

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		public string ContentType
		{
			get;
			set;
		}

        /// <summary>Gets the headers.</summary>
        ///
        /// <value>The headers.</value>
		public NameValueCollection Headers
		{
			get;
			private set;
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
			this.Headers.Add(name, value);
		}

        /// <summary>Redirects the given document.</summary>
        ///
        /// <param name="url">URL of the document.</param>
		public void Redirect(string url)
		{
			this.Headers.Add(HttpHeaders.Location, url.MapServerPath());
		}

        /// <summary>Gets the output stream.</summary>
        ///
        /// <value>The output stream.</value>
		public Stream OutputStream
		{
			get;
			private set;
		}

        /// <summary>Writes.</summary>
        ///
        /// <param name="text">The text to write.</param>
		public void Write(string text)
		{
			this.TextWritten.Append(text);
		}

        /// <summary>
        /// Signal that this response has been handled and no more processing should be done. When used in a request or response filter, no more filters or processing is done on this request.
        /// </summary>
		public void Close()
		{
			this.IsClosed = true;
			OutputStream.Position = 0;
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
	        Headers[HttpHeaders.ContentLength] = contentLength.ToString(CultureInfo.InvariantCulture);
	    }
	}
}