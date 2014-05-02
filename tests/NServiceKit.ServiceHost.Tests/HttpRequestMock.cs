using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceKit.Common.Utils;

namespace NServiceKit.ServiceHost.Tests
{
	class HttpRequestMock : IHttpRequest
	{
        /// <summary>The underlying ASP.NET or HttpListener HttpRequest.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The original request.</value>
		public object OriginalRequest
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>The name of the service being called (e.g. Request DTO Name)</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The name of the operation.</value>
		public string OperationName
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

        /// <summary>The request ContentType.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The type of the content.</value>
		public string ContentType
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets a value indicating whether this object is local.</summary>
        ///
        /// <value>true if this object is local, false if not.</value>
        public bool IsLocal
        {
            get { return true; }
        }

        /// <summary>Gets the HTTP method.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The HTTP method.</value>
		public string HttpMethod
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets the user agent.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The user agent.</value>
		public string UserAgent
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets the cookies.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The cookies.</value>
		public IDictionary<string, System.Net.Cookie> Cookies
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>The expected Response ContentType for this request.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The type of the response content.</value>
		public string ResponseContentType
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

        /// <summary>Attach any data to this request that all filters and services can access.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The items.</value>
		public Dictionary<string, object> Items
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets the headers.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The headers.</value>
		public System.Collections.Specialized.NameValueCollection Headers
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets the query string.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The query string.</value>
		public System.Collections.Specialized.NameValueCollection QueryString
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets information describing the form.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>Information describing the form.</value>
		public System.Collections.Specialized.NameValueCollection FormData
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Buffer the Request InputStream so it can be re-read.</summary>
        ///
        /// <value>true if use buffered stream, false if not.</value>
	    public bool UseBufferedStream { get; set; }

        /// <summary>The entire string contents of Request.InputStream.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <returns>The raw body.</returns>
	    public string GetRawBody()
		{
			throw new NotImplementedException();
		}

        /// <summary>Gets URL of the raw.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The raw URL.</value>
		public string RawUrl
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets URI of the absolute.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The absolute URI.</value>
		public string AbsoluteUri
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>The Remote Ip as reported by Request.UserHostAddress.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The user host address.</value>
		public string UserHostAddress
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>The Remote Ip as reported by X-Forwarded-For, X-Real-IP or Request.UserHostAddress.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The remote IP.</value>
		public string RemoteIp
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>The value of the X-Forwarded-For header, null if null or empty.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The x coordinate forwarded for.</value>
        public string XForwardedFor
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>The value of the X-Real-IP header, null if null or empty.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The x coordinate real IP.</value>
        public string XRealIp
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>e.g. is https or not.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>true if this object is secure connection, false if not.</value>
		public bool IsSecureConnection
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets a list of types of the accepts.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>A list of types of the accepts.</value>
		public string[] AcceptTypes
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
		public string PathInfo
		{
			get { return "index.html"; }
		}

        /// <summary>Gets the input stream.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The input stream.</value>
		public System.IO.Stream InputStream
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets the length of the content.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The length of the content.</value>
		public long ContentLength
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Access to the multi-part/formdata files posted on this request.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The files.</value>
		public IFile[] Files
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>Gets the full pathname of the application file.</summary>
        ///
        /// <value>The full pathname of the application file.</value>
		public string ApplicationFilePath
		{
			get { return "~".MapAbsolutePath(); }
		}

        /// <summary>Try resolve.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
		public T TryResolve<T>()
		{
			throw new NotImplementedException();
		}

        /// <summary>The value of the Referrer, null if not available.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The URL referrer.</value>
        public Uri UrlReferrer
        {
            get { throw new NotImplementedException(); }
        }
    }
}
