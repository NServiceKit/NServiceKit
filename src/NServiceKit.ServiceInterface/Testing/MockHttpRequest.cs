using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Funq;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface.Testing
{
    /// <summary>A mock HTTP request.</summary>
    public class MockHttpRequest : IHttpRequest
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.MockHttpRequest class.</summary>
        public MockHttpRequest()
        {
            this.FormData = new NameValueCollection();
            this.Headers = new NameValueCollection();
            this.QueryString = new NameValueCollection();
            this.Cookies = new Dictionary<string, Cookie>();
            this.Items = new Dictionary<string, object>();
            this.Container = new Container();
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.MockHttpRequest class.</summary>
        ///
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="httpMethod">   The HTTP method.</param>
        /// <param name="contentType">  The type of the content.</param>
        /// <param name="pathInfo">     Information describing the path.</param>
        /// <param name="queryString">  The query string.</param>
        /// <param name="inputStream">  The input stream.</param>
        /// <param name="formData">     Information describing the form.</param>
        public MockHttpRequest(string operationName, string httpMethod,
            string contentType, string pathInfo,
            NameValueCollection queryString, Stream inputStream, NameValueCollection formData)
            : this()
        {
            this.OperationName = operationName;
            this.HttpMethod = httpMethod;
            this.ContentType = contentType;
            this.ResponseContentType = contentType;
            this.PathInfo = pathInfo;
            this.InputStream = inputStream;
            this.QueryString = queryString;
            this.FormData = formData ?? new NameValueCollection();
        }

        /// <summary>The underlying ASP.NET or HttpListener HttpRequest.</summary>
        ///
        /// <value>The original request.</value>
        public object OriginalRequest
        {
            get { return null; }
        }

        /// <summary>Try resolve.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public T TryResolve<T>()
        {
            return Container.TryResolve<T>();
        }

        /// <summary>Gets or sets the container.</summary>
        ///
        /// <value>The container.</value>
        public Container Container { get; set; }

        /// <summary>The name of the service being called (e.g. Request DTO Name)</summary>
        ///
        /// <value>The name of the operation.</value>
        public string OperationName { get; set; }

        /// <summary>The request ContentType.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>Gets or sets the HTTP method.</summary>
        ///
        /// <value>The HTTP method.</value>
        public string HttpMethod { get; set; }

        /// <summary>Gets or sets the user agent.</summary>
        ///
        /// <value>The user agent.</value>
        public string UserAgent { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is local.</summary>
        ///
        /// <value>true if this object is local, false if not.</value>
        public bool IsLocal { get; set; }

        /// <summary>Gets or sets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
        public IDictionary<string, Cookie> Cookies { get; set; }

        private string responseContentType;

        /// <summary>The expected Response ContentType for this request.</summary>
        ///
        /// <value>The type of the response content.</value>
        public string ResponseContentType
        {
            get { return responseContentType ?? this.ContentType; }
            set { responseContentType = value; }
        }

        /// <summary>Gets or sets the headers.</summary>
        ///
        /// <value>The headers.</value>
        public NameValueCollection Headers { get; set; }

        /// <summary>Gets or sets the query string.</summary>
        ///
        /// <value>The query string.</value>
        public NameValueCollection QueryString { get; set; }

        /// <summary>Gets or sets information describing the form.</summary>
        ///
        /// <value>Information describing the form.</value>
        public NameValueCollection FormData { get; set; }

        /// <summary>Buffer the Request InputStream so it can be re-read.</summary>
        ///
        /// <value>true if use buffered stream, false if not.</value>
        public bool UseBufferedStream { get; set; }

        /// <summary>Attach any data to this request that all filters and services can access.</summary>
        ///
        /// <value>The items.</value>
        public Dictionary<string, object> Items
        {
            get;
            private set;
        }

        private string rawBody;

        /// <summary>The entire string contents of Request.InputStream.</summary>
        ///
        /// <returns>The raw body.</returns>
        public string GetRawBody()
        {
            if (rawBody != null) return rawBody;
            if (InputStream == null) return null;

            //Keep the stream alive in-case it needs to be read twice (i.e. ContentLength)
            rawBody = new StreamReader(InputStream).ReadToEnd();
            InputStream.Position = 0;
            return rawBody;
        }

        /// <summary>Gets or sets URL of the raw.</summary>
        ///
        /// <value>The raw URL.</value>
        public string RawUrl { get; set; }

        /// <summary>Gets or sets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
        public string AbsoluteUri
        {
            get { return "http://localhost" + this.PathInfo; }
        }

        /// <summary>The Remote Ip as reported by Request.UserHostAddress.</summary>
        ///
        /// <value>The user host address.</value>
        public string UserHostAddress { get; set; }

        /// <summary>The Remote Ip as reported by X-Forwarded-For, X-Real-IP or Request.UserHostAddress.</summary>
        ///
        /// <value>The remote IP.</value>
        public string RemoteIp { get; set; }

        /// <summary>The value of the X-Forwarded-For header, null if null or empty.</summary>
        ///
        /// <value>The x coordinate forwarded for.</value>
        public string XForwardedFor { get; set; }

        /// <summary>The value of the X-Real-IP header, null if null or empty.</summary>
        ///
        /// <value>The x coordinate real IP.</value>
        public string XRealIp { get; set; }

        /// <summary>e.g. is https or not.</summary>
        ///
        /// <value>true if this object is secure connection, false if not.</value>
        public bool IsSecureConnection { get; set; }

        /// <summary>Gets or sets a list of types of the accepts.</summary>
        ///
        /// <value>A list of types of the accepts.</value>
        public string[] AcceptTypes { get; set; }

        /// <summary>Gets or sets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
        public string PathInfo { get; set; }

        /// <summary>Gets or sets the input stream.</summary>
        ///
        /// <value>The input stream.</value>
        public Stream InputStream { get; set; }

        /// <summary>Gets the length of the content.</summary>
        ///
        /// <value>The length of the content.</value>
        public long ContentLength
        {
            get
            {
                var body = GetRawBody();
                return body != null ? body.Length : 0;
            }
        }

        /// <summary>Access to the multi-part/formdata files posted on this request.</summary>
        ///
        /// <value>The files.</value>
        public IFile[] Files { get; set; }

        /// <summary>Gets or sets the full pathname of the application file.</summary>
        ///
        /// <value>The full pathname of the application file.</value>
        public string ApplicationFilePath { get; set; }

        /// <summary>Adds session cookies.</summary>
        public void AddSessionCookies()
        {
            var permSessionId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            this.Cookies[SessionFeature.PermanentSessionId] = new Cookie(SessionFeature.PermanentSessionId, permSessionId);
            var sessionId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            this.Cookies[SessionFeature.SessionId] = new Cookie(SessionFeature.SessionId, sessionId);
        }

        /// <summary>The value of the Referrer, null if not available.</summary>
        ///
        /// <value>The URL referrer.</value>
        public Uri UrlReferrer { get { return null; } }
    }
}