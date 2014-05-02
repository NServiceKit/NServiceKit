using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Linq;
#if !(MONOTOUCH || SILVERLIGHT)
using System.Text;
using System.Web;
#endif
#if !SILVERLIGHT
using System.Reflection;
#endif
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.Net30.Collections.Concurrent;
using NServiceKit.Service;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.ServiceClient.Web
{

    /**
     * Need to provide async request options
     * http://msdn.microsoft.com/en-us/library/86wf6409(VS.71).aspx
     */
    public abstract class ServiceClientBase
#if !SILVERLIGHT
 : IServiceClient, IRestClient
#else
        : IServiceClient
#endif
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServiceClientBase));

        private string replyPath = "/syncreply/";
        private string oneWayPath = "/asynconeway/";

        private AuthenticationInfo authInfo = null;

        /// <summary>Sets a value indicating whether this object use new predefined routes.</summary>
        ///
        /// <value>true if use new predefined routes, false if not.</value>
        public bool UseNewPredefinedRoutes
        {
            set
            {
                replyPath = value ? "/reply/" : "/syncreply/";
                oneWayPath = value ? "/oneway/" : "/asynconeway/";
                this.SyncReplyBaseUri = BaseUri.WithTrailingSlash() + Format + replyPath;
                this.AsyncOneWayBaseUri = BaseUri.WithTrailingSlash() + Format + oneWayPath;
            }
        }

        /// <summary>
        /// The request filter is called before any request.
        /// This request filter is executed globally.
        /// </summary>
        private static Action<HttpWebRequest> httpWebRequestFilter;

        /// <summary>Gets or sets the HTTP web request filter.</summary>
        ///
        /// <value>The HTTP web request filter.</value>
        public static Action<HttpWebRequest> HttpWebRequestFilter
        {
            get
            {
                return httpWebRequestFilter;
            }
            set
            {
                httpWebRequestFilter = value;
                AsyncServiceClient.HttpWebRequestFilter = value;
            }
        }

        /// <summary>
        /// The response action is called once the server response is available.
        /// It will allow you to access raw response information. 
        /// This response action is executed globally.
        /// Note that you should NOT consume the response stream as this is handled by NServiceKit
        /// </summary>
        private static Action<HttpWebResponse> httpWebResponseFilter;

        /// <summary>Gets or sets the HTTP web response filter.</summary>
        ///
        /// <value>The HTTP web response filter.</value>
        public static Action<HttpWebResponse> HttpWebResponseFilter
        {
            get
            {
                return httpWebResponseFilter;
            }
            set
            {
                httpWebResponseFilter = value;
                AsyncServiceClient.HttpWebResponseFilter = value;
            }
        }

#if NETFX_CORE || WINDOWS_PHONE || SILVERLIGHT
        /// <summary>
        /// Gets the collection of headers to be added to outgoing requests.
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; } 
#else

        /// <summary>Gets the headers.</summary>
        ///
        /// <value>The headers.</value>
        public NameValueCollection Headers { get; private set; }
#endif

        /// <summary>The default HTTP method.</summary>
        public const string DefaultHttpMethod = "POST";

        readonly AsyncServiceClient asyncClient;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.ServiceClientBase class.</summary>
        protected ServiceClientBase()
        {
            this.HttpMethod = DefaultHttpMethod;
            asyncClient = new AsyncServiceClient
            {
                ContentType = ContentType,
                StreamSerializer = SerializeToStream,
                StreamDeserializer = StreamDeserializer,
                UserName = this.UserName,
                Password = this.Password,
                LocalHttpWebRequestFilter = this.LocalHttpWebRequestFilter,
                LocalHttpWebResponseFilter = this.LocalHttpWebResponseFilter
            };
            this.CookieContainer = new CookieContainer();
            this.StoreCookies = true; //leave
#if NETFX_CORE || WINDOWS_PHONE || SILVERLIGHT
            this.Headers = new Dictionary<string, string>();
#else
            this.Headers = new NameValueCollection();
#endif

#if SILVERLIGHT
            asyncClient.HandleCallbackOnUIThread = this.HandleCallbackOnUIThread = true;
            asyncClient.UseBrowserHttpHandling = this.UseBrowserHttpHandling = false;
            asyncClient.ShareCookiesWithBrowser = this.ShareCookiesWithBrowser = true;
#endif
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.ServiceClientBase class.</summary>
        ///
        /// <param name="syncReplyBaseUri">  The synchronise reply base URI.</param>
        /// <param name="asyncOneWayBaseUri">The asynchronous one way base URI.</param>
        protected ServiceClientBase(string syncReplyBaseUri, string asyncOneWayBaseUri)
            : this()
        {
            this.SyncReplyBaseUri = syncReplyBaseUri;
            this.AsyncOneWayBaseUri = asyncOneWayBaseUri;
        }

        /// <summary>
        /// Sets all baseUri properties, using the Format property for the SyncReplyBaseUri and AsyncOneWayBaseUri
        /// </summary>
        /// <param name="baseUri">Base URI of the service</param>
        public void SetBaseUri(string baseUri)
        {
            this.BaseUri = baseUri;
            this.asyncClient.BaseUri = baseUri;
            this.SyncReplyBaseUri = baseUri.WithTrailingSlash() + Format + replyPath;
            this.AsyncOneWayBaseUri = baseUri.WithTrailingSlash() + Format + oneWayPath;
        }

        /// <summary>
        /// Sets all baseUri properties allowing for a temporary override of the Format property
        /// </summary>
        /// <param name="baseUri">Base URI of the service</param>
        /// <param name="format">Override of the Format property for the service</param>
        //Marked obsolete on 4/11/2012
        [Obsolete("Please call the SetBaseUri(string baseUri) method, which uses the specific implementation's Format property.")]
        public void SetBaseUri(string baseUri, string format)
        {
            this.BaseUri = baseUri;
            this.asyncClient.BaseUri = baseUri;
            this.SyncReplyBaseUri = baseUri.WithTrailingSlash() + format + "/syncreply/";
            this.AsyncOneWayBaseUri = baseUri.WithTrailingSlash() + format + "/asynconeway/";
        }

        /// <summary>
        /// Whether to Accept Gzip,Deflate Content-Encoding and to auto decompress responses
        /// </summary>
        private bool disableAutoCompression;

        /// <summary>Gets or sets a value indicating whether the automatic compression is disabled.</summary>
        ///
        /// <value>true if disable automatic compression, false if not.</value>
        public bool DisableAutoCompression
        {
            get { return disableAutoCompression; }
            set
            {
                disableAutoCompression = value;
                asyncClient.DisableAutoCompression = value;
            }
        }

        /// <summary>
        /// The user name for basic authentication
        /// </summary>
        private string username;

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        public string UserName
        {
            get { return username; }
            set
            {
                username = value;
                asyncClient.UserName = value;
            }
        }

        /// <summary>
        /// The password for basic authentication
        /// </summary>
        private string password;

        /// <summary>Gets or sets the password.</summary>
        ///
        /// <value>The password.</value>
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                asyncClient.Password = value;
            }
        }

        /// <summary>
        /// Sets the username and the password for basic authentication.
        /// </summary>
        public void SetCredentials(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }

        /// <summary>Gets or sets URI of the base.</summary>
        ///
        /// <value>The base URI.</value>
        public string BaseUri { get; set; }

        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public abstract string Format { get; }

        /// <summary>Gets or sets URI of the synchronise reply base.</summary>
        ///
        /// <value>The synchronise reply base URI.</value>
        public string SyncReplyBaseUri { get; set; }

        /// <summary>Gets or sets URI of the asynchronous one way base.</summary>
        ///
        /// <value>The asynchronous one way base URI.</value>
        public string AsyncOneWayBaseUri { get; set; }

        private TimeSpan? timeout;

        /// <summary>Gets or sets the timeout.</summary>
        ///
        /// <value>The timeout.</value>
        public TimeSpan? Timeout
        {
            get { return this.timeout; }
            set
            {
                this.timeout = value;
                this.asyncClient.Timeout = value;
            }
        }

        private TimeSpan? readWriteTimeout;

        /// <summary>Gets or sets the read write timeout.</summary>
        ///
        /// <value>The read write timeout.</value>
        public TimeSpan? ReadWriteTimeout
        {
            get { return this.readWriteTimeout; }
            set
            {
                this.readWriteTimeout = value;
                // TODO implement ReadWriteTimeout in asyncClient
                //this.asyncClient.ReadWriteTimeout = value;
            }
        }

        /// <summary>Gets the accept.</summary>
        ///
        /// <value>The accept.</value>
        public virtual string Accept
        {
            get { return ContentType; }
        }

        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public abstract string ContentType { get; }

        /// <summary>Gets or sets the HTTP method.</summary>
        ///
        /// <value>The HTTP method.</value>
        public string HttpMethod { get; set; }

#if !SILVERLIGHT

        /// <summary>Gets or sets the proxy.</summary>
        ///
        /// <value>The proxy.</value>
        public IWebProxy Proxy { get; set; }
#endif

#if SILVERLIGHT
        private bool handleCallbackOnUiThread;
        public bool HandleCallbackOnUIThread
        {
            get { return this.handleCallbackOnUiThread; }
            set { asyncClient.HandleCallbackOnUIThread = this.handleCallbackOnUiThread = value; }
        }

        private bool useBrowserHttpHandling;
        public bool UseBrowserHttpHandling
        {
            get { return this.useBrowserHttpHandling; }
            set { asyncClient.UseBrowserHttpHandling = this.useBrowserHttpHandling = value; }
        }

        private bool shareCookiesWithBrowser;
        public bool ShareCookiesWithBrowser
        {
            get { return this.shareCookiesWithBrowser; }
            set { asyncClient.ShareCookiesWithBrowser = this.shareCookiesWithBrowser = value; }
        }

#endif

        private ICredentials credentials;

        /// <summary>
        /// Gets or sets authentication information for the request.
        /// Warning: It's recommened to use <see cref="UserName"/> and <see cref="Password"/> for basic auth.
        /// This property is only used for IIS level authentication.
        /// </summary>
        public ICredentials Credentials
        {
            set
            {
                this.credentials = value;
                this.asyncClient.Credentials = value;
            }
        }

        /// <summary>
        /// Determines if the basic auth header should be sent with every request.
        /// By default, the basic auth header is only sent when "401 Unauthorized" is returned.
        /// </summary>
        private bool alwaysSendBasicAuthHeader;

        /// <summary>Gets or sets a value indicating whether the always send basic authentication header.</summary>
        ///
        /// <value>true if always send basic authentication header, false if not.</value>
        public bool AlwaysSendBasicAuthHeader
        {
            get { return alwaysSendBasicAuthHeader; }
            set { asyncClient.AlwaysSendBasicAuthHeader = alwaysSendBasicAuthHeader = value; }
        }

        /// <summary>
        /// Specifies if cookies should be stored
        /// </summary>
        private bool storeCookies;

        /// <summary>Gets or sets a value indicating whether the store cookies.</summary>
        ///
        /// <value>true if store cookies, false if not.</value>
        public bool StoreCookies
        {
            get { return storeCookies; }
            set { asyncClient.StoreCookies = storeCookies = value; }
        }

        private CookieContainer _cookieContainer;

        /// <summary>Gets or sets the cookie container.</summary>
        ///
        /// <value>The cookie container.</value>
        public CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
            set { asyncClient.CookieContainer = _cookieContainer = value; }
        }

        private bool allowAutoRedirect = true;

        /// <summary>Gets or sets a value indicating whether we allow automatic redirect.</summary>
        ///
        /// <value>true if allow automatic redirect, false if not.</value>
        public bool AllowAutoRedirect
        {
            get { return allowAutoRedirect; }
            set
            {
                allowAutoRedirect = value;
                // TODO: Implement for async client.
                // asyncClient.AllowAutoRedirect = value;
            }
        }

        /// <summary>
        /// Called before request resend, when the initial request required authentication
        /// </summary>
        private Action<WebRequest> onAuthenticationRequired { get; set; }

        /// <summary>Gets or sets the on authentication required.</summary>
        ///
        /// <value>The on authentication required.</value>
        public Action<WebRequest> OnAuthenticationRequired
        {
            get
            {
                return onAuthenticationRequired;
            }
            set
            {
                onAuthenticationRequired = value;
                asyncClient.OnAuthenticationRequired = value;
            }
        }

        /// <summary>
        /// The request filter is called before any request.
        /// This request filter only works with the instance where it was set (not global).
        /// </summary>
        private Action<HttpWebRequest> localHttpWebRequestFilter { get; set; }

        /// <summary>Gets or sets the local HTTP web request filter.</summary>
        ///
        /// <value>The local HTTP web request filter.</value>
        public Action<HttpWebRequest> LocalHttpWebRequestFilter
        {
            get
            {
                return localHttpWebRequestFilter;
            }
            set
            {
                localHttpWebRequestFilter = value;
                asyncClient.LocalHttpWebRequestFilter = value;
            }
        }

        /// <summary>
        /// The response action is called once the server response is available.
        /// It will allow you to access raw response information. 
        /// Note that you should NOT consume the response stream as this is handled by NServiceKit
        /// </summary>
        private Action<HttpWebResponse> localHttpWebResponseFilter { get; set; }

        /// <summary>Gets or sets the local HTTP web response filter.</summary>
        ///
        /// <value>The local HTTP web response filter.</value>
        public Action<HttpWebResponse> LocalHttpWebResponseFilter
        {
            get
            {
                return localHttpWebResponseFilter;
            }
            set
            {
                localHttpWebResponseFilter = value;
                asyncClient.LocalHttpWebResponseFilter = value;
            }
        }

        /// <summary>Serialize to stream.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        /// <param name="stream">        The stream.</param>
        public abstract void SerializeToStream(IRequestContext requestContext, object request, Stream stream);

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>A T.</returns>
        public abstract T DeserializeFromStream<T>(Stream stream);

        /// <summary>Gets the stream deserializer.</summary>
        ///
        /// <value>The stream deserializer.</value>
        public abstract StreamDeserializerDelegate StreamDeserializer { get; }

#if !SILVERLIGHT

        /// <summary>Send this message.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Send<TResponse>(IReturn<TResponse> request)
        {
            return Send<TResponse>((object)request);
        }

        /// <summary>Send this message.</summary>
        ///
        /// <param name="request">The request to get.</param>
        public virtual void Send(IReturnVoid request)
        {
            SendOneWay(request);
        }

        /// <summary>Send this message.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Send<TResponse>(object request)
        {
            var requestUri = this.SyncReplyBaseUri.WithTrailingSlash() + request.GetType().Name;
            var client = SendRequest(requestUri, request);

            try
            {
                var webResponse = client.GetResponse();
                return HandleResponse<TResponse>(webResponse);
            }
            catch (Exception ex)
            {
                TResponse response;

                if (!HandleResponseException(ex,
                    request,
                    requestUri,
                    () => SendRequest(HttpMethods.Post, requestUri, request),
                    c => c.GetResponse(),
                    out response))
                {
                    throw;
                }

                return response;
            }
        }

        /// <summary>
        /// Called by Send method if an exception occurs, for instance a System.Net.WebException because the server
        /// returned an HTTP error code. Override if you want to handle specific exceptions or always want to parse the
        /// response to a custom ErrorResponse DTO type instead of NServiceKit's ErrorResponse class. In case ex is a
        /// <c>System.Net.WebException</c>, do not use
        /// <c>createWebRequest</c>/<c>getResponse</c>/<c>HandleResponse&lt;TResponse&gt;</c> to parse the response
        /// because that will result in the same exception again. Use
        /// <c>ThrowWebServiceException&lt;YourErrorResponseType&gt;</c> to parse the response and to throw a
        /// <c>WebServiceException</c> containing the parsed DTO. Then override Send to handle that exception.
        /// </summary>
        protected virtual bool HandleResponseException<TResponse>(Exception ex, object request, string requestUri,
            Func<WebRequest> createWebRequest, Func<WebRequest, WebResponse> getResponse, out TResponse response)
        {
            try
            {
                if (WebRequestUtils.ShouldAuthenticate(ex, this.UserName, this.Password))
                {
                    // adamfowleruk : Check response object to see what type of auth header to add

                    var client = createWebRequest();

                    var webEx = ex as WebException;
                    if (webEx != null && webEx.Response != null)
                    {
                        WebHeaderCollection headers = ((HttpWebResponse)webEx.Response).Headers;
                        var doAuthHeader = headers[NServiceKit.Common.Web.HttpHeaders.WwwAuthenticate];
                        // check value of WWW-Authenticate header
                        if (doAuthHeader == null)
                        {
                            client.AddBasicAuth(this.UserName, this.Password);
                        }
                        else
                        {
                            this.authInfo = new NServiceKit.ServiceClient.Web.AuthenticationInfo(doAuthHeader);
                            client.AddAuthInfo(this.UserName, this.Password, authInfo);
                        }
                    }


                    if (OnAuthenticationRequired != null)
                    {
                        OnAuthenticationRequired(client);
                    }

                    var webResponse = getResponse(client);

                    response = HandleResponse<TResponse>(webResponse);
                    return true;
                }
            }
            catch (Exception subEx)
            {
                // Since we are effectively re-executing the call, 
                // the new exception should be shown to the caller rather
                // than the old one.
                // The new exception is either this one or the one thrown
                // by the following method.
                ThrowResponseTypeException<TResponse>(request, subEx, requestUri);
                throw;
            }

            // If this doesn't throw, the calling method 
            // should rethrow the original exception upon
            // return value of false.
            ThrowResponseTypeException<TResponse>(request, ex, requestUri);

            response = default(TResponse);
            return false;
        }

        readonly ConcurrentDictionary<Type, Action<Exception, string>> ResponseHandlers
            = new ConcurrentDictionary<Type, Action<Exception, string>>();

        private void ThrowResponseTypeException<TResponse>(object request, Exception ex, string requestUri)
        {
            if (request == null)
            {
                ThrowWebServiceException<TResponse>(ex, requestUri);
                return;
            }

            var responseType = WebRequestUtils.GetErrorResponseDtoType(request);
            Action<Exception, string> responseHandler;
            if (!ResponseHandlers.TryGetValue(responseType, out responseHandler))
            {
                var mi = GetType().GetMethod("ThrowWebServiceException",
                        BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(new[] { responseType });

                responseHandler = (Action<Exception, string>)Delegate.CreateDelegate(
                    typeof(Action<Exception, string>), this, mi);

                ResponseHandlers[responseType] = responseHandler;
            }
            responseHandler(ex, requestUri);
        }

        /// <summary>Throw web service exception.</summary>
        ///
        /// <exception cref="WebServiceException">  Thrown when a Web Service error condition occurs.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="ex">        The ex.</param>
        /// <param name="requestUri">URI of the request.</param>
        protected internal void ThrowWebServiceException<TResponse>(Exception ex, string requestUri)
        {
            var webEx = ex as WebException;
            if (webEx != null && webEx.Status == WebExceptionStatus.ProtocolError)
            {
                var errorResponse = ((HttpWebResponse)webEx.Response);
                log.Error(webEx);
                log.DebugFormat("Status Code : {0}", errorResponse.StatusCode);
                log.DebugFormat("Status Description : {0}", errorResponse.StatusDescription);

                var serviceEx = new WebServiceException(errorResponse.StatusDescription)
                {
                    StatusCode = (int)errorResponse.StatusCode,
                    StatusDescription = errorResponse.StatusDescription,
                };

                try
                {
                    if (errorResponse.ContentType.MatchesContentType(ContentType))
                    {
                        var bytes = errorResponse.GetResponseStream().ReadFully();
                        using (var stream = new MemoryStream(bytes))
                        {
                            serviceEx.ResponseBody = bytes.FromUtf8Bytes();
                            serviceEx.ResponseDto = DeserializeFromStream<TResponse>(stream);
                        }
                    }
                    else
                    {
                        serviceEx.ResponseBody = errorResponse.GetResponseStream().ToUtf8String();
                    }
                }
                catch (Exception innerEx)
                {
                    // Oh, well, we tried
                    throw new WebServiceException(errorResponse.StatusDescription, innerEx)
                    {
                        StatusCode = (int)errorResponse.StatusCode,
                        StatusDescription = errorResponse.StatusDescription,
                        ResponseBody = serviceEx.ResponseBody
                    };
                }

                //Escape deserialize exception handling and throw here
                throw serviceEx;
            }

            var authEx = ex as AuthenticationException;
            if (authEx != null)
            {
                throw WebRequestUtils.CreateCustomException(requestUri, authEx);
            }
        }

        private WebRequest SendRequest(string requestUri, object request)
        {
            return SendRequest(HttpMethod ?? DefaultHttpMethod, requestUri, request);
        }

        private WebRequest SendRequest(string httpMethod, string requestUri, object request)
        {
            return PrepareWebRequest(httpMethod, requestUri, request, client =>
            {
                using (var requestStream = client.GetRequestStream())
                {
                    SerializeToStream(null, request, requestStream);
                }
            });
        }

        private WebRequest PrepareWebRequest(string httpMethod, string requestUri, object request, Action<HttpWebRequest> sendRequestAction)
        {
            if (httpMethod == null)
                throw new ArgumentNullException("httpMethod");

            var httpMethodGetOrHead = httpMethod == HttpMethods.Get || httpMethod == HttpMethods.Head;

            if (httpMethodGetOrHead && request != null)
            {
                var queryString = QueryStringSerializer.SerializeToString(request);
                if (!string.IsNullOrEmpty(queryString))
                {
                    requestUri += "?" + queryString;
                }
            }

            var client = (HttpWebRequest)WebRequest.Create(requestUri);

            try
            {
                client.Accept = Accept;
                client.Method = httpMethod;
                client.Headers.Add(Headers);

                if (Proxy != null) client.Proxy = Proxy;
                if (this.Timeout.HasValue) client.Timeout = (int)this.Timeout.Value.TotalMilliseconds;
                if (this.ReadWriteTimeout.HasValue) client.ReadWriteTimeout = (int)this.ReadWriteTimeout.Value.TotalMilliseconds;
                if (this.credentials != null) client.Credentials = this.credentials;

                if (null != this.authInfo)
                {
                    client.AddAuthInfo(this.UserName, this.Password, authInfo);
                }
                else
                {
                    if (this.AlwaysSendBasicAuthHeader) client.AddBasicAuth(this.UserName, this.Password);
                }

                if (!DisableAutoCompression)
                {
                    client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                    client.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }

                if (StoreCookies)
                {
                    client.CookieContainer = CookieContainer;
                }

                client.AllowAutoRedirect = AllowAutoRedirect;

                ApplyWebRequestFilters(client);

                if (httpMethod != HttpMethods.Get
                    && httpMethod != HttpMethods.Delete
                    && httpMethod != HttpMethods.Head)
                {
                    client.ContentType = ContentType;

                    if (sendRequestAction != null) sendRequestAction(client);
                }
            }
            catch (AuthenticationException ex)
            {
                throw WebRequestUtils.CreateCustomException(requestUri, ex) ?? ex;
            }
            return client;
        }

        private void ApplyWebResponseFilters(WebResponse webResponse)
        {
            if (!(webResponse is HttpWebResponse)) return;

            if (HttpWebResponseFilter != null)
                HttpWebResponseFilter((HttpWebResponse)webResponse);
            if (LocalHttpWebResponseFilter != null)
                LocalHttpWebResponseFilter((HttpWebResponse)webResponse);
        }

        private void ApplyWebRequestFilters(HttpWebRequest client)
        {
            if (LocalHttpWebRequestFilter != null)
                LocalHttpWebRequestFilter(client);

            if (HttpWebRequestFilter != null)
                HttpWebRequestFilter(client);
        }
#else
        private void SendRequest(string requestUri, object request, Action<WebRequest> callback)
        {
            var isHttpGet = HttpMethod != null && HttpMethod.ToUpper() == "GET";
            if (isHttpGet)
            {
                var queryString = QueryStringSerializer.SerializeToString(request);
                if (!string.IsNullOrEmpty(queryString))
                {
                    requestUri += "?" + queryString;
                }
            }

            SendRequest(HttpMethod ?? DefaultHttpMethod, requestUri, request, callback);
        }

        private void SendRequest(string httpMethod, string requestUri, object request, Action<WebRequest> callback)
        {
            if (httpMethod == null)
                throw new ArgumentNullException("httpMethod");

            var client = (HttpWebRequest)WebRequest.Create(requestUri);
            try
            {
                client.Accept = ContentType;
                client.Method = httpMethod;

                if (this.credentials != null) client.Credentials = this.credentials;
                if (this.AlwaysSendBasicAuthHeader) client.AddBasicAuth(this.UserName, this.Password);

                if (StoreCookies)
                {
                    client.CookieContainer = CookieContainer;
                }

                if (this.LocalHttpWebRequestFilter != null)
                    LocalHttpWebRequestFilter(client);

                if (HttpWebRequestFilter != null)
                    HttpWebRequestFilter(client);

                if (httpMethod != HttpMethods.Get
                    && httpMethod != HttpMethods.Delete)
                {
                    client.ContentType = ContentType;

                    client.BeginGetRequestStream(delegate(IAsyncResult target)
                    {
                        var webReq = (HttpWebRequest)target.AsyncState;
                        var requestStream = webReq.EndGetRequestStream(target);
                        SerializeToStream(null, request, requestStream);
                        callback(client);
                    }, null);
                }
            }
            catch (AuthenticationException ex)
            {
                throw WebRequestUtils.CreateCustomException(requestUri, ex) ?? ex;
            }
        }
#endif

        /// <summary>Gets an URL.</summary>
        ///
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        ///
        /// <returns>The URL.</returns>
        public virtual string GetUrl(string relativeOrAbsoluteUrl)
        {
            return relativeOrAbsoluteUrl.StartsWith("http:")
                || relativeOrAbsoluteUrl.StartsWith("https:")
                     ? relativeOrAbsoluteUrl
                     : this.BaseUri.CombineWith(relativeOrAbsoluteUrl);
        }

#if !SILVERLIGHT
        private byte[] DownloadBytes(string httpMethod, string requestUri, object request)
        {
            var webRequest = SendRequest(httpMethod, requestUri, request);
            using (var response = webRequest.GetResponse())
            {
                ApplyWebResponseFilters(response);
                using (var stream = response.GetResponseStream())
                    return stream.ReadFully();
            }
        }
#else
        private void DownloadBytes(string httpMethod, string requestUri, object request, Action<byte[]> callback = null)
        {
            SendRequest(httpMethod, requestUri, request, webRequest => webRequest.BeginGetResponse(delegate(IAsyncResult result)
            {
                var webReq = (HttpWebRequest)result.AsyncState;
                var response = (HttpWebResponse)webReq.EndGetResponse(result);
                using (var stream = response.GetResponseStream())
                {
                    var bytes = stream.ReadFully();
                    if (callback != null)
                    {
                        callback(bytes);
                    }
                }
            }, null));
        }
#endif

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="request">The request.</param>
        public virtual void SendOneWay(object request)
        {
            var requestUri = this.AsyncOneWayBaseUri.WithTrailingSlash() + request.GetType().Name;
            DownloadBytes(HttpMethods.Post, requestUri, request);
        }

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        public virtual void SendOneWay(string relativeOrAbsoluteUrl, object request)
        {
            var requestUri = GetUrl(relativeOrAbsoluteUrl);
            DownloadBytes(HttpMethods.Post, requestUri, request);
        }

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="httpMethod">           The HTTP method.</param>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        public virtual void SendOneWay(string httpMethod, string relativeOrAbsoluteUrl, object request)
        {
            var requestUri = GetUrl(relativeOrAbsoluteUrl);
            DownloadBytes(httpMethod, requestUri, request);
        }

        /// <summary>Sends the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public virtual void SendAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            SendAsync((object)request, onSuccess, onError);
        }

        /// <summary>Sends the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public virtual void SendAsync<TResponse>(object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            var requestUri = this.SyncReplyBaseUri.WithTrailingSlash() + request.GetType().Name;
            asyncClient.SendAsync(HttpMethods.Post, requestUri, request, onSuccess, onError);
        }

        /// <summary>Gets the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public virtual void GetAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            GetAsync(request.ToUrl(HttpMethods.Get, Format), onSuccess, onError);
        }

        /// <summary>Gets the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public virtual void GetAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            asyncClient.SendAsync(HttpMethods.Get, GetUrl(relativeOrAbsoluteUrl), null, onSuccess, onError);
        }

        /// <summary>Deletes the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public virtual void DeleteAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            DeleteAsync(request.ToUrl(HttpMethods.Delete, Format), onSuccess, onError);
        }

        /// <summary>Deletes the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public virtual void DeleteAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            asyncClient.SendAsync(HttpMethods.Delete, GetUrl(relativeOrAbsoluteUrl), null, onSuccess, onError);
        }

        /// <summary>Posts the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public virtual void PostAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            PostAsync(request.ToUrl(HttpMethods.Post, Format), request, onSuccess, onError);
        }

        /// <summary>Posts the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public virtual void PostAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            asyncClient.SendAsync(HttpMethods.Post, GetUrl(relativeOrAbsoluteUrl), request, onSuccess, onError);
        }

        /// <summary>Puts the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public virtual void PutAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            PutAsync(request.ToUrl(HttpMethods.Put, Format), request, onSuccess, onError);
        }

        /// <summary>Puts the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public virtual void PutAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            asyncClient.SendAsync(HttpMethods.Put, GetUrl(relativeOrAbsoluteUrl), request, onSuccess, onError);
        }

        /// <summary>Patch asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public virtual void PatchAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            PatchAsync(request.ToUrl(HttpMethods.Patch, Format), request, onSuccess, onError);
        }

        /// <summary>Patch asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public virtual void PatchAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            asyncClient.SendAsync(HttpMethods.Patch, GetUrl(relativeOrAbsoluteUrl), request, onSuccess, onError);
        }

        /// <summary>Custom method asynchronous.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="httpVerb"> The HTTP verb.</param>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public virtual void CustomMethodAsync<TResponse>(string httpVerb, IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            if (!HttpMethods.HasVerb(httpVerb))
                throw new NotSupportedException("Unknown HTTP Method is not supported: " + httpVerb);

            asyncClient.SendAsync(httpVerb, GetUrl(request.ToUrl(httpVerb, Format)), request, onSuccess, onError);
        }

        /// <summary>Cancel asynchronous.</summary>
        public virtual void CancelAsync()
        {
            asyncClient.CancelAsync();
        }

#if !SILVERLIGHT

        /// <summary>Send this message.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="httpMethod">           The HTTP method.</param>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Send<TResponse>(string httpMethod, string relativeOrAbsoluteUrl, object request)
        {
            var requestUri = GetUrl(relativeOrAbsoluteUrl);
            var client = SendRequest(httpMethod, requestUri, request);

            try
            {
                var webResponse = client.GetResponse();
                return HandleResponse<TResponse>(webResponse);
            }
            catch (Exception ex)
            {
                TResponse response;

                if (!HandleResponseException(
                    ex,
                    request,
                    requestUri,
                    () => SendRequest(httpMethod, requestUri, request),
                    c => c.GetResponse(),
                    out response))
                {
                    throw;
                }

                return response;
            }
        }

        /// <summary>Gets.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Get<TResponse>(IReturn<TResponse> request)
        {
            return Send<TResponse>(HttpMethods.Get, request.ToUrl(HttpMethods.Get, Format), null);
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        public virtual void Get(IReturnVoid request)
        {
            SendOneWay(HttpMethods.Get, request.ToUrl(HttpMethods.Get, Format), null);
        }

        /// <summary>Gets.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Get<TResponse>(string relativeOrAbsoluteUrl)
        {
            return Send<TResponse>(HttpMethods.Get, relativeOrAbsoluteUrl, null);
        }

        /// <summary>Deletes the given relativeOrAbsoluteUrl.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Delete<TResponse>(IReturn<TResponse> request)
        {
            return Send<TResponse>(HttpMethods.Delete, request.ToUrl(HttpMethods.Delete, Format), null);
        }

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        public virtual void Delete(IReturnVoid request)
        {
            SendOneWay(HttpMethods.Delete, request.ToUrl(HttpMethods.Delete, Format), null);
        }

        /// <summary>Deletes the given relativeOrAbsoluteUrl.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Delete<TResponse>(string relativeOrAbsoluteUrl)
        {
            return Send<TResponse>(HttpMethods.Delete, relativeOrAbsoluteUrl, null);
        }

        /// <summary>Post this message.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Post<TResponse>(IReturn<TResponse> request)
        {
            return Send<TResponse>(HttpMethods.Post, request.ToUrl(HttpMethods.Post, Format), request);
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        public virtual void Post(IReturnVoid request)
        {
            SendOneWay(HttpMethods.Post, request.ToUrl(HttpMethods.Post, Format), request);
        }

        /// <summary>Post this message.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Post<TResponse>(string relativeOrAbsoluteUrl, object request)
        {
            return Send<TResponse>(HttpMethods.Post, relativeOrAbsoluteUrl, request);
        }

        /// <summary>Puts.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Put<TResponse>(IReturn<TResponse> request)
        {
            return Send<TResponse>(HttpMethods.Put, request.ToUrl(HttpMethods.Put, Format), request);
        }

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request to put.</param>
        public virtual void Put(IReturnVoid request)
        {
            SendOneWay(HttpMethods.Put, request.ToUrl(HttpMethods.Put, Format), request);
        }

        /// <summary>Puts.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Put<TResponse>(string relativeOrAbsoluteUrl, object request)
        {
            return Send<TResponse>(HttpMethods.Put, relativeOrAbsoluteUrl, request);
        }

        /// <summary>Patches.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Patch<TResponse>(IReturn<TResponse> request)
        {
            return Send<TResponse>(HttpMethods.Patch, request.ToUrl(HttpMethods.Patch, Format), request);
        }

        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        public virtual void Patch(IReturnVoid request)
        {
            SendOneWay(HttpMethods.Patch, request.ToUrl(HttpMethods.Patch, Format), request);
        }

        /// <summary>Patches.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse Patch<TResponse>(string relativeOrAbsoluteUrl, object request)
        {
            return Send<TResponse>(HttpMethods.Patch, relativeOrAbsoluteUrl, request);
        }

        /// <summary>Custom method.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="httpVerb">The HTTP verb.</param>
        /// <param name="request"> The request.</param>
        public virtual void CustomMethod(string httpVerb, IReturnVoid request)
        {
            if (!HttpMethods.AllVerbs.Contains(httpVerb.ToUpper()))
                throw new NotSupportedException("Unknown HTTP Method is not supported: " + httpVerb);

            SendOneWay(httpVerb, request.ToUrl(httpVerb, Format), request);
        }

        /// <summary>Custom method.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="httpVerb">The HTTP verb.</param>
        /// <param name="request"> The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse CustomMethod<TResponse>(string httpVerb, IReturn<TResponse> request)
        {
            if (!HttpMethods.AllVerbs.Contains(httpVerb.ToUpper()))
                throw new NotSupportedException("Unknown HTTP Method is not supported: " + httpVerb);

            return Send<TResponse>(httpVerb, request.ToUrl(httpVerb, Format), request);
        }

        /// <summary>Heads the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpWebResponse.</returns>
        public virtual HttpWebResponse Head(IReturn request)
        {
            return Send<HttpWebResponse>(HttpMethods.Head, request.ToUrl(HttpMethods.Head), request);
        }

        /// <summary>Heads.</summary>
        ///
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        ///
        /// <returns>A HttpWebResponse.</returns>
        public virtual HttpWebResponse Head(string relativeOrAbsoluteUrl)
        {
            return Send<HttpWebResponse>(HttpMethods.Head, relativeOrAbsoluteUrl, null);
        }

        /// <summary>Posts a file with request.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse PostFileWithRequest<TResponse>(string relativeOrAbsoluteUrl, FileInfo fileToUpload, object request)
        {
            using (FileStream fileStream = fileToUpload.OpenRead())
            {
                return PostFileWithRequest<TResponse>(relativeOrAbsoluteUrl, fileStream, fileToUpload.Name, request);

            }
        }

        /// <summary>Posts a file with request.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="fileName">             Filename of the file.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse PostFileWithRequest<TResponse>(string relativeOrAbsoluteUrl, Stream fileToUpload, string fileName, object request)
        {
            var requestUri = GetUrl(relativeOrAbsoluteUrl);
            var currentStreamPosition = fileToUpload.Position;

            Func<WebRequest> createWebRequest = () =>
            {
                var webRequest = PrepareWebRequest(HttpMethods.Post, requestUri, null, null);

                var queryString = QueryStringSerializer.SerializeToString(request);
#if !MONOTOUCH
                var nameValueCollection = HttpUtility.ParseQueryString(queryString);
#endif
                var boundary = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                boundary = "--" + boundary;
                var newLine = "\r\n";
                using (var outputStream = webRequest.GetRequestStream())
                {
#if !MONOTOUCH
                    foreach (var key in nameValueCollection.AllKeys)
                    {
                        outputStream.Write(boundary + newLine);
                        outputStream.Write("Content-Disposition: form-data;name=\"{0}\"{1}".FormatWith(key, newLine));
                        outputStream.Write("Content-Type: text/plain;charset=utf-8{0}{1}".FormatWith(newLine, newLine));
                        outputStream.Write(nameValueCollection[key] + newLine);
                    }
#endif
                    outputStream.Write(boundary + newLine);
                    outputStream.Write("Content-Disposition: form-data;name=\"{0}\";filename=\"{1}\"{2}{3}".FormatWith("upload", fileName, newLine, newLine));
                    var buffer = new byte[4096];
                    int byteCount;
                    while ((byteCount = fileToUpload.Read(buffer, 0, 4096)) > 0)
                    {
                        outputStream.Write(buffer, 0, byteCount);
                    }
                    outputStream.Write(newLine);
                    outputStream.Write(boundary + "--");
                }

                return webRequest;
            };

            try
            {
                var webRequest = createWebRequest();
                var webResponse = webRequest.GetResponse();
                return HandleResponse<TResponse>(webResponse);
            }
            catch (Exception ex)
            {
                TResponse response;

                // restore original position before retry
                fileToUpload.Seek(currentStreamPosition, SeekOrigin.Begin);

                if (!HandleResponseException(
                    ex, request, requestUri, createWebRequest, c => c.GetResponse(), out response))
                {
                    throw;
                }

                return response;
            }
        }

        /// <summary>Posts a file.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="mimeType">             Type of the mime.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse PostFile<TResponse>(string relativeOrAbsoluteUrl, FileInfo fileToUpload, string mimeType)
        {
            using (FileStream fileStream = fileToUpload.OpenRead())
            {
                return PostFile<TResponse>(relativeOrAbsoluteUrl, fileStream, fileToUpload.Name, mimeType);
            }
        }

        /// <summary>Posts a file.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="fileName">             Filename of the file.</param>
        /// <param name="mimeType">             Type of the mime.</param>
        ///
        /// <returns>A TResponse.</returns>
        public virtual TResponse PostFile<TResponse>(string relativeOrAbsoluteUrl, Stream fileToUpload, string fileName, string mimeType)
        {
            var currentStreamPosition = fileToUpload.Position;
            var requestUri = GetUrl(relativeOrAbsoluteUrl);
            Func<WebRequest> createWebRequest = () => PrepareWebRequest(HttpMethods.Post, requestUri, null, null);

            try
            {
                var webRequest = createWebRequest();
                webRequest.UploadFile(fileToUpload, fileName, mimeType);
                var webResponse = webRequest.GetResponse();
                return HandleResponse<TResponse>(webResponse);
            }
            catch (Exception ex)
            {
                TResponse response;

                // restore original position before retry
                fileToUpload.Seek(currentStreamPosition, SeekOrigin.Begin);

                if (!HandleResponseException(ex,
                    null,
                    requestUri,
                    createWebRequest,
                    c => { c.UploadFile(fileToUpload, fileName, mimeType); return c.GetResponse(); },
                    out response))
                {
                    throw;
                }

                return response;
            }
        }

        private TResponse HandleResponse<TResponse>(WebResponse webResponse)
        {
            ApplyWebResponseFilters(webResponse);

            if (typeof(TResponse) == typeof(HttpWebResponse) && (webResponse is HttpWebResponse))
            {
                return (TResponse)Convert.ChangeType(webResponse, typeof(TResponse));
            }
            if (typeof(TResponse) == typeof(Stream)) //Callee Needs to dispose manually
            {
                return (TResponse)(object)webResponse.GetResponseStream();
            }

            using (var responseStream = webResponse.GetResponseStream())
            {
                if (typeof(TResponse) == typeof(string))
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        return (TResponse)(object)reader.ReadToEnd();
                    }
                }
                if (typeof(TResponse) == typeof(byte[]))
                {
                    return (TResponse)(object)responseStream.ReadFully();
                }

                var response = DeserializeFromStream<TResponse>(responseStream);
                return response;
            }
        }

#endif

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() { }
    }
}
