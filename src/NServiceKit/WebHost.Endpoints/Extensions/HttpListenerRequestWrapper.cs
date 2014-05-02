using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Funq;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Extensions
{
    /// <summary>A HTTP listener request wrapper.</summary>
    public partial class HttpListenerRequestWrapper
        : IHttpRequest
    {
        private static readonly string physicalFilePath;
        private readonly HttpListenerRequest request;

        /// <summary>Gets or sets the container.</summary>
        ///
        /// <value>The container.</value>
        public Container Container { get; set; }

        static HttpListenerRequestWrapper()
        {
            physicalFilePath = "~".MapAbsolutePath();
        }

        /// <summary>Gets the request.</summary>
        ///
        /// <value>The request.</value>
        public HttpListenerRequest Request
        {
            get { return request; }
        }

        /// <summary>The underlying ASP.NET or HttpListener HttpRequest.</summary>
        ///
        /// <value>The original request.</value>
        public object OriginalRequest
        {
            get { return request; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Extensions.HttpListenerRequestWrapper class.</summary>
        ///
        /// <param name="request">The request.</param>
        public HttpListenerRequestWrapper(HttpListenerRequest request)
            : this(null, request) { }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Extensions.HttpListenerRequestWrapper class.</summary>
        ///
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="request">      The request.</param>
        public HttpListenerRequestWrapper(
            string operationName, HttpListenerRequest request)
        {
            this.OperationName = operationName;
            this.request = request;
        }

        /// <summary>Try resolve.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public T TryResolve<T>()
        {
            return Container == null
                ? EndpointHost.AppHost.TryResolve<T>()
                : Container.TryResolve<T>();
        }

        /// <summary>The name of the service being called (e.g. Request DTO Name)</summary>
        ///
        /// <value>The name of the operation.</value>
        public string OperationName { get; set; }

        /// <summary>The entire string contents of Request.InputStream.</summary>
        ///
        /// <returns>The raw body.</returns>
        public string GetRawBody()
        {
            if (bufferedStream != null)
            {
                return bufferedStream.ToArray().FromUtf8Bytes();
            }

            using (var reader = new StreamReader(InputStream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>Gets URL of the raw.</summary>
        ///
        /// <value>The raw URL.</value>
        public string RawUrl
        {
            get { return request.RawUrl; }
        }

        /// <summary>Gets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
        public string AbsoluteUri
        {
            get { return request.Url.AbsoluteUri.TrimEnd('/'); }
        }

        /// <summary>The Remote Ip as reported by Request.UserHostAddress.</summary>
        ///
        /// <value>The user host address.</value>
        public string UserHostAddress
        {
            get { return request.UserHostAddress; }
        }

        /// <summary>The value of the X-Forwarded-For header, null if null or empty.</summary>
        ///
        /// <value>The x coordinate forwarded for.</value>
        public string XForwardedFor
        {
            get
            {
                return String.IsNullOrEmpty(request.Headers[HttpHeaders.XForwardedFor]) ? null : request.Headers[HttpHeaders.XForwardedFor];
            }
        }

        /// <summary>The value of the X-Real-IP header, null if null or empty.</summary>
        ///
        /// <value>The x coordinate real IP.</value>
        public string XRealIp
        {
            get
            {
                return String.IsNullOrEmpty(request.Headers[HttpHeaders.XRealIp]) ? null : request.Headers[HttpHeaders.XRealIp];
            }
        }

        private string remoteIp;

        /// <summary>The Remote Ip as reported by X-Forwarded-For, X-Real-IP or Request.UserHostAddress.</summary>
        ///
        /// <value>The remote IP.</value>
        public string RemoteIp
        {
            get
            {
                return remoteIp ??
                    (remoteIp = XForwardedFor ??
                                (XRealIp ??
                                ((request.RemoteEndPoint != null) ? request.RemoteEndPoint.Address.ToString() : null)));
            }
        }

        /// <summary>e.g. is https or not.</summary>
        ///
        /// <value>true if this object is secure connection, false if not.</value>
        public bool IsSecureConnection
        {
            get { return request.IsSecureConnection; }
        }

        /// <summary>Gets a list of types of the accepts.</summary>
        ///
        /// <value>A list of types of the accepts.</value>
        public string[] AcceptTypes
        {
            get { return request.AcceptTypes; }
        }

        private Dictionary<string, object> items;

        /// <summary>Attach any data to this request that all filters and services can access.</summary>
        ///
        /// <value>The items.</value>
        public Dictionary<string, object> Items
        {
            get { return items ?? (items = new Dictionary<string, object>()); }
        }

        private string responseContentType;

        /// <summary>The expected Response ContentType for this request.</summary>
        ///
        /// <value>The type of the response content.</value>
        public string ResponseContentType
        {
            get { return responseContentType ?? (responseContentType = this.GetResponseContentType()); }
            set { this.responseContentType = value; }
        }

        private string pathInfo;

        /// <summary>Gets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
        public string PathInfo
        {
            get
            {
                if (this.pathInfo == null)
                {
                    var mode = EndpointHost.Config.NServiceKitHandlerFactoryPath;

                    var pos = request.RawUrl.IndexOf("?");
                    if (pos != -1)
                    {
                        var path = request.RawUrl.Substring(0, pos);
                        this.pathInfo = HttpRequestExtensions.GetPathInfo(
                            path,
                            mode,
                            mode ?? "");
                    }
                    else
                    {
                        this.pathInfo = request.RawUrl;
                    }

                    this.pathInfo = this.pathInfo.UrlDecode();
                    this.pathInfo = NormalizePathInfo(pathInfo, mode);
                }
                return this.pathInfo;
            }
        }

        private Dictionary<string, Cookie> cookies;

        /// <summary>Gets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
        public IDictionary<string, Cookie> Cookies
        {
            get
            {
                if (cookies == null)
                {
                    cookies = new Dictionary<string, Cookie>();
                    for (var i = 0; i < this.request.Cookies.Count; i++)
                    {
                        var httpCookie = this.request.Cookies[i];
                        cookies[httpCookie.Name] = httpCookie;
                    }
                }

                return cookies;
            }
        }

        /// <summary>Gets the user agent.</summary>
        ///
        /// <value>The user agent.</value>
        public string UserAgent
        {
            get { return request.UserAgent; }
        }

        /// <summary>Gets the headers.</summary>
        ///
        /// <value>The headers.</value>
        public NameValueCollection Headers
        {
            get { return request.Headers; }
        }

        private NameValueCollection queryString;

        /// <summary>Gets the query string.</summary>
        ///
        /// <value>The query string.</value>
        public NameValueCollection QueryString
        {
            get { return queryString ?? (queryString = HttpUtility.ParseQueryString(request.Url.Query)); }
        }

        /// <summary>Gets information describing the form.</summary>
        ///
        /// <value>Information describing the form.</value>
        public NameValueCollection FormData
        {
            get { return this.Form; }
        }

        /// <summary>Gets a value indicating whether this object is local.</summary>
        ///
        /// <value>true if this object is local, false if not.</value>
        public bool IsLocal
        {
            get { return request.IsLocal; }
        }

        private string httpMethod;

        /// <summary>Gets the HTTP method.</summary>
        ///
        /// <value>The HTTP method.</value>
        public string HttpMethod
        {
            get
            {
                return httpMethod
                    ?? (httpMethod = Param(HttpHeaders.XHttpMethodOverride)
                    ?? request.HttpMethod);
            }
        }

        /// <summary>Parameters.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>A string.</returns>
        public string Param(string name)
        {
            return Headers[name]
                ?? QueryString[name]
                ?? FormData[name];
        }

        /// <summary>The request ContentType.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get { return request.ContentType; }
        }

        /// <summary>Gets the content encoding.</summary>
        ///
        /// <value>The content encoding.</value>
        public Encoding ContentEncoding
        {
            get { return request.ContentEncoding; }
        }

        /// <summary>The value of the Referrer, null if not available.</summary>
        ///
        /// <value>The URL referrer.</value>
        public Uri UrlReferrer
        {
            get { return request.UrlReferrer; }
        }

        /// <summary>Gets an encoding.</summary>
        ///
        /// <param name="contentTypeHeader">The content type header.</param>
        ///
        /// <returns>The encoding.</returns>
        public static Encoding GetEncoding(string contentTypeHeader)
        {
            var param = GetParameter(contentTypeHeader, "charset=");
            if (param == null) return null;
            try
            {
                return Encoding.GetEncoding(param);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>Buffer the Request InputStream so it can be re-read.</summary>
        ///
        /// <value>true if use buffered stream, false if not.</value>
        public bool UseBufferedStream
        {
            get { return bufferedStream != null; }
            set
            {
                bufferedStream = value
                    ? bufferedStream ?? new MemoryStream(request.InputStream.ReadFully())
                    : null;
            }
        }

        private MemoryStream bufferedStream;

        /// <summary>Gets the input stream.</summary>
        ///
        /// <value>The input stream.</value>
        public Stream InputStream
        {
            get { return bufferedStream ?? request.InputStream; }
        }

        /// <summary>Gets the length of the content.</summary>
        ///
        /// <value>The length of the content.</value>
        public long ContentLength
        {
            get { return request.ContentLength64; }
        }

        /// <summary>Gets the full pathname of the application file.</summary>
        ///
        /// <value>The full pathname of the application file.</value>
        public string ApplicationFilePath
        {
            get { return physicalFilePath; }
        }

        private IFile[] _files;

        /// <summary>Access to the multi-part/formdata files posted on this request.</summary>
        ///
        /// <value>The files.</value>
        public IFile[] Files
        {
            get
            {
                if (_files == null)
                {
                    if (files == null)
                        return _files = new IFile[0];

                    _files = new IFile[files.Count];
                    for (var i = 0; i < files.Count; i++)
                    {
                        var reqFile = files[i];

                        _files[i] = new HttpFile
                        {
                            ContentType = reqFile.ContentType,
                            ContentLength = reqFile.ContentLength,
                            FileName = reqFile.FileName,
                            InputStream = reqFile.InputStream,
                        };
                    }
                }
                return _files;
            }
        }

        static Stream GetSubStream(Stream stream)
        {
            if (stream is MemoryStream)
            {
                var other = (MemoryStream)stream;
                try
                {
                    return new MemoryStream(other.GetBuffer(), 0, (int)other.Length, false, true);
                }
                catch (UnauthorizedAccessException)
                {
                    return new MemoryStream(other.ToArray(), 0, (int)other.Length, false, true);
                }
            }

            return stream;
        }

        static void EndSubStream(Stream stream)
        {
        }

        /// <summary>Gets handler path if any.</summary>
        ///
        /// <param name="listenerUrl">URL of the listener.</param>
        ///
        /// <returns>The handler path if any.</returns>
        public static string GetHandlerPathIfAny(string listenerUrl)
        {
            if (listenerUrl == null) return null;
            var pos = listenerUrl.IndexOf("://", StringComparison.InvariantCultureIgnoreCase);
            if (pos == -1) return null;
            var startHostUrl = listenerUrl.Substring(pos + "://".Length);
            var endPos = startHostUrl.IndexOf('/');
            if (endPos == -1) return null;
            var endHostUrl = startHostUrl.Substring(endPos + 1);
            return String.IsNullOrEmpty(endHostUrl) ? null : endHostUrl.TrimEnd('/');
        }

        /// <summary>Normalize path information.</summary>
        ///
        /// <param name="pathInfo">   Information describing the path.</param>
        /// <param name="handlerPath">Full pathname of the handler file.</param>
        ///
        /// <returns>A string.</returns>
        public static string NormalizePathInfo(string pathInfo, string handlerPath)
        {
            if (handlerPath != null && pathInfo.TrimStart('/').StartsWith(
                handlerPath, StringComparison.InvariantCultureIgnoreCase))
            {
                return pathInfo.TrimStart('/').Substring(handlerPath.Length);
            }

            return pathInfo;
        }
    }

}
