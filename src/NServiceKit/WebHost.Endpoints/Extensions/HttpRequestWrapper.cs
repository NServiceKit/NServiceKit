using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using Funq;
using NServiceKit.Text;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Extensions
{
    /// <summary>A HTTP request wrapper.</summary>
    public class HttpRequestWrapper
        : IHttpRequest
    {
        private static readonly string physicalFilePath;

        /// <summary>Gets or sets the container.</summary>
        ///
        /// <value>The container.</value>
        public Container Container { get; set; }
        private readonly HttpRequest request;

        static HttpRequestWrapper()
        {
            physicalFilePath = "~".MapHostAbsolutePath();
        }

        /// <summary>Gets the request.</summary>
        ///
        /// <value>The request.</value>
        public HttpRequest Request
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

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Extensions.HttpRequestWrapper class.</summary>
        ///
        /// <param name="request">The request.</param>
        public HttpRequestWrapper(HttpRequest request)
            : this(null, request)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Extensions.HttpRequestWrapper class.</summary>
        ///
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="request">      The request.</param>
        public HttpRequestWrapper(string operationName, HttpRequest request)
        {
            this.OperationName = operationName;
            this.request = request;
            this.Container = Container;
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

        /// <summary>The request ContentType.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get { return request.ContentType; }
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

        /// <summary>Gets a value indicating whether this object is local.</summary>
        ///
        /// <value>true if this object is local, false if not.</value>
        public bool IsLocal
        {
            get { return request.IsLocal; }
        }

        /// <summary>Gets the user agent.</summary>
        ///
        /// <value>The user agent.</value>
        public string UserAgent
        {
            get { return request.UserAgent; }
        }

        private Dictionary<string, object> items;

        /// <summary>Attach any data to this request that all filters and services can access.</summary>
        ///
        /// <value>The items.</value>
        public Dictionary<string, object> Items
        {
            get
            {
                if (items == null)
                {
                    items = new Dictionary<string, object>();
                }
                return items;
            }
        }

        private string responseContentType;

        /// <summary>The expected Response ContentType for this request.</summary>
        ///
        /// <value>The type of the response content.</value>
        public string ResponseContentType
        {
            get
            {
                if (responseContentType == null)
                {
                    responseContentType = this.GetResponseContentType();
                }
                return responseContentType;
            }
            set
            {
                this.responseContentType = value;
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
                        Cookie cookie = null;

                        // try-catch needed as malformed cookie names (e.g. '$Version') can be returned
                        // from Cookie.Name, but the Cookie constructor will throw for these names.
                        try
                        {
                            cookie = new Cookie(httpCookie.Name, httpCookie.Value, httpCookie.Path, httpCookie.Domain)
                            {
                                HttpOnly = httpCookie.HttpOnly,
                                Secure = httpCookie.Secure,
                                Expires = httpCookie.Expires,
                            };
                        }
                        catch
                        {
                            // I don't like this, application code now has access to less data than it would when
                            // using the raw HttpRequest. Atleast logging that here would be nice?
                            // Not sure, leaving it up to you Demis.
                        }

                        if ( cookie != null )
                            cookies[httpCookie.Name] = cookie;

                    }
                }
                return cookies;
            }
        }

        /// <summary>Gets the headers.</summary>
        ///
        /// <value>The headers.</value>
        public NameValueCollection Headers
        {
            get { return request.Headers; }
        }

        /// <summary>Gets the query string.</summary>
        ///
        /// <value>The query string.</value>
        public NameValueCollection QueryString
        {
            get { return request.QueryString; }
        }

        /// <summary>Gets information describing the form.</summary>
        ///
        /// <value>Information describing the form.</value>
        public NameValueCollection FormData
        {
            get { return request.Form; }
        }

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
            get
            {
                try
                {
                    return request.Url.AbsoluteUri.TrimEnd('/');
                }
                catch (Exception)
                {
                    //fastcgi mono, do a 2nd rounds best efforts
                    return "http://" + request.UserHostName + request.RawUrl;
                }
            }
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
                return string.IsNullOrEmpty(request.Headers[HttpHeaders.XForwardedFor]) ? null : request.Headers[HttpHeaders.XForwardedFor];
            }
        }

        /// <summary>The value of the X-Real-IP header, null if null or empty.</summary>
        ///
        /// <value>The x coordinate real IP.</value>
        public string XRealIp
        {
            get
            {
                return string.IsNullOrEmpty(request.Headers[HttpHeaders.XRealIp]) ? null : request.Headers[HttpHeaders.XRealIp];
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
                return remoteIp ?? (remoteIp = XForwardedFor ?? (XRealIp ?? request.UserHostAddress));
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

        /// <summary>Gets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
        public string PathInfo
        {
            get { return request.GetPathInfo(); }
        }

        /// <summary>Gets the name of the URL host.</summary>
        ///
        /// <value>The name of the URL host.</value>
        public string UrlHostName
        {
            get { return request.GetUrlHostName(); }
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
            get { return request.ContentLength; }
        }

        private IFile[] files;

        /// <summary>Access to the multi-part/formdata files posted on this request.</summary>
        ///
        /// <value>The files.</value>
        public IFile[] Files
        {
            get
            {
                if (files == null)
                {
                    files = new IFile[request.Files.Count];
                    for (var i = 0; i < request.Files.Count; i++)
                    {
                        var reqFile = request.Files[i];

                        files[i] = new HttpFile
                        {
                            ContentType = reqFile.ContentType,
                            ContentLength = reqFile.ContentLength,
                            FileName = reqFile.FileName,
                            InputStream = reqFile.InputStream,
                        };
                    }
                }
                return files;
            }
        }

        /// <summary>Gets the full pathname of the application file.</summary>
        ///
        /// <value>The full pathname of the application file.</value>
        public string ApplicationFilePath
        {
            get { return physicalFilePath; }
        }

        /// <summary>The value of the Referrer, null if not available.</summary>
        ///
        /// <value>The URL referrer.</value>
        public Uri UrlReferrer
        {
            get { return request.UrlReferrer; }
        }
    }

}