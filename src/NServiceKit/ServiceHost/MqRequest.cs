using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using NServiceKit.Common.Web;
using NServiceKit.ServiceModel;
using NServiceKit.Text;

namespace NServiceKit.ServiceHost
{
    /// <summary>A mq request.</summary>
    public class MqRequest : IHttpRequest
    {
        private readonly MqRequestContext requestContext;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.MqRequest class.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        public MqRequest(MqRequestContext requestContext)
        {
            this.requestContext = requestContext;
        }

        /// <summary>Try resolve.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public T TryResolve<T>()
        {
            return requestContext.Resolver.TryResolve<T>();
        }

        /// <summary>The underlying ASP.NET or HttpListener HttpRequest.</summary>
        ///
        /// <value>The original request.</value>
        public object OriginalRequest
        {
            get { return requestContext.Message; }
        }

        /// <summary>The name of the service being called (e.g. Request DTO Name)</summary>
        ///
        /// <value>The name of the operation.</value>
        public string OperationName
        {
            get { return requestContext.OperationName; }
            set { requestContext.OperationName = value; }
        }

        /// <summary>The request ContentType.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get { return requestContext.ContentType; }
        }

        /// <summary>Gets the HTTP method.</summary>
        ///
        /// <value>The HTTP method.</value>
        public string HttpMethod
        {
            get { return HttpMethods.Post; }
        }

        /// <summary>Gets the user agent.</summary>
        ///
        /// <value>The user agent.</value>
        public string UserAgent
        {
            get { return "MQ"; }
        }

        /// <summary>Gets a value indicating whether this object is local.</summary>
        ///
        /// <value>true if this object is local, false if not.</value>
        public bool IsLocal
        {
            get { return true; }
        }

        /// <summary>Gets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
        public IDictionary<string, Cookie> Cookies
        {
            get { return requestContext.Cookies; }
        }

        /// <summary>The expected Response ContentType for this request.</summary>
        ///
        /// <value>The type of the response content.</value>
        public string ResponseContentType
        {
            get { return requestContext.ResponseContentType; }
            set { requestContext.ResponseContentType = value; }
        }

        private Dictionary<string, object> items;

        /// <summary>Attach any data to this request that all filters and services can access.</summary>
        ///
        /// <value>The items.</value>
        public Dictionary<string, object> Items
        {
            get { return items ?? (items = new Dictionary<string, object>()); }
        }

        private NameValueCollection headers;

        /// <summary>Gets the headers.</summary>
        ///
        /// <value>The headers.</value>
        public NameValueCollection Headers
        {
            get { return headers ?? (headers = requestContext.Headers.ToNameValueCollection()); }
        }

        /// <summary>Gets the query string.</summary>
        ///
        /// <value>The query string.</value>
        public NameValueCollection QueryString
        {
            get { return new NameValueCollection(); }
        }

        /// <summary>Gets information describing the form.</summary>
        ///
        /// <value>Information describing the form.</value>
        public NameValueCollection FormData
        {
            get { return new NameValueCollection(); }
        }

        /// <summary>Buffer the Request InputStream so it can be re-read.</summary>
        ///
        /// <value>true if use buffered stream, false if not.</value>
        public bool UseBufferedStream { get; set; }

        private string body;

        /// <summary>The entire string contents of Request.InputStream.</summary>
        ///
        /// <returns>The raw body.</returns>
        public string GetRawBody()
        {
            return body ?? (body = (requestContext.Message.Body ?? "").Dump());
        }

        /// <summary>Gets URL of the raw.</summary>
        ///
        /// <value>The raw URL.</value>
        public string RawUrl
        {
            get { return "mq://" + requestContext.PathInfo; }
        }

        /// <summary>Gets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
        public string AbsoluteUri
        {
            get { return "mq://" + requestContext.PathInfo; }
        }

        /// <summary>The Remote Ip as reported by Request.UserHostAddress.</summary>
        ///
        /// <value>The user host address.</value>
        public string UserHostAddress
        {
            get { return null; }
        }

        /// <summary>The Remote Ip as reported by X-Forwarded-For, X-Real-IP or Request.UserHostAddress.</summary>
        ///
        /// <value>The remote IP.</value>
        public string RemoteIp
        {
            get { return null; }
        }

        /// <summary>The value of the X-Forwarded-For header, null if null or empty.</summary>
        ///
        /// <value>The x coordinate forwarded for.</value>
        public string XForwardedFor
        {
            get { return null; }
        }

        /// <summary>The value of the X-Real-IP header, null if null or empty.</summary>
        ///
        /// <value>The x coordinate real IP.</value>
        public string XRealIp 
        {
            get { return null; }
        }

        /// <summary>e.g. is https or not.</summary>
        ///
        /// <value>true if this object is secure connection, false if not.</value>
        public bool IsSecureConnection
        {
            get { return false; }
        }

        /// <summary>Gets a list of types of the accepts.</summary>
        ///
        /// <value>A list of types of the accepts.</value>
        public string[] AcceptTypes
        {
            get { return new string[0]; }
        }

        /// <summary>Gets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
        public string PathInfo
        {
            get { return requestContext.PathInfo; }
        }

        /// <summary>Gets the input stream.</summary>
        ///
        /// <value>The input stream.</value>
        public Stream InputStream
        {
            get { return null; }
        }

        /// <summary>Gets the length of the content.</summary>
        ///
        /// <value>The length of the content.</value>
        public long ContentLength
        {
            get { return (GetRawBody() ?? "").Length; }
        }

        /// <summary>Access to the multi-part/formdata files posted on this request.</summary>
        ///
        /// <value>The files.</value>
        public IFile[] Files
        {
            get { return null; }
        }

        /// <summary>Gets the full pathname of the application file.</summary>
        ///
        /// <value>The full pathname of the application file.</value>
        public string ApplicationFilePath
        {
            get { return null; }
        }

        /// <summary>The value of the Referrer, null if not available.</summary>
        ///
        /// <value>The URL referrer.</value>
        public System.Uri UrlReferrer
        {
            get { return null; }
        }
    }
}