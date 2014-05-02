#if !SILVERLIGHT && !XBOX
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace NServiceKit.ServiceHost
{
    /// <summary>
    /// A thin wrapper around ASP.NET or HttpListener's HttpRequest
    /// </summary>
    public interface IHttpRequest : IResolver
    {
        /// <summary>
        /// The underlying ASP.NET or HttpListener HttpRequest
        /// </summary>
        object OriginalRequest { get; }

        /// <summary>
        /// The name of the service being called (e.g. Request DTO Name)
        /// </summary>
        string OperationName { get; set; }

        /// <summary>
        /// The request ContentType
        /// </summary>
        string ContentType { get; }

        /// <summary>Gets the HTTP method.</summary>
        ///
        /// <value>The HTTP method.</value>
        string HttpMethod { get; }

        /// <summary>Gets a value indicating whether this object is local.</summary>
        ///
        /// <value>true if this object is local, false if not.</value>
        bool IsLocal { get; }

        /// <summary>Gets the user agent.</summary>
        ///
        /// <value>The user agent.</value>
        string UserAgent { get; }

        /// <summary>Gets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
        IDictionary<string, System.Net.Cookie> Cookies { get; }

        /// <summary>
        /// The expected Response ContentType for this request
        /// </summary>
        string ResponseContentType { get; set; }

        /// <summary>
        /// Attach any data to this request that all filters and services can access.
        /// </summary>
        Dictionary<string, object> Items { get; }

        /// <summary>Gets the headers.</summary>
        ///
        /// <value>The headers.</value>
        NameValueCollection Headers { get; }

        /// <summary>Gets the query string.</summary>
        ///
        /// <value>The query string.</value>
        NameValueCollection QueryString { get; }

        /// <summary>Gets information describing the form.</summary>
        ///
        /// <value>Information describing the form.</value>
        NameValueCollection FormData { get; }

        /// <summary>
        /// Buffer the Request InputStream so it can be re-read
        /// </summary>
        bool UseBufferedStream { get; set; }

        /// <summary>
        /// The entire string contents of Request.InputStream
        /// </summary>
        /// <returns></returns>
        string GetRawBody();

        /// <summary>Gets URL of the raw.</summary>
        ///
        /// <value>The raw URL.</value>
        string RawUrl { get; }

        /// <summary>Gets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
        string AbsoluteUri { get; }

        /// <summary>
        /// The Remote Ip as reported by Request.UserHostAddress
        /// </summary>
        string UserHostAddress { get; }

        /// <summary>
        /// The Remote Ip as reported by X-Forwarded-For, X-Real-IP or Request.UserHostAddress
        /// </summary>
        string RemoteIp { get; }

        /// <summary>
        /// The value of the X-Forwarded-For header, null if null or empty
        /// </summary>
        string XForwardedFor { get; }

        /// <summary>
        /// The value of the X-Real-IP header, null if null or empty
        /// </summary>
        string XRealIp { get; }

        /// <summary>
        /// e.g. is https or not
        /// </summary>
        bool IsSecureConnection { get; }

        /// <summary>Gets a list of types of the accepts.</summary>
        ///
        /// <value>A list of types of the accepts.</value>
        string[] AcceptTypes { get; }

        /// <summary>Gets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
        string PathInfo { get; }

        /// <summary>Gets the input stream.</summary>
        ///
        /// <value>The input stream.</value>
        Stream InputStream { get; }

        /// <summary>Gets the length of the content.</summary>
        ///
        /// <value>The length of the content.</value>
        long ContentLength { get; }

        /// <summary>
        /// Access to the multi-part/formdata files posted on this request
        /// </summary>
        IFile[] Files { get; }

        /// <summary>Gets the full pathname of the application file.</summary>
        ///
        /// <value>The full pathname of the application file.</value>
        string ApplicationFilePath { get; }

        /// <summary>
        /// The value of the Referrer, null if not available
        /// </summary>
        Uri UrlReferrer { get; }
    }
}
#endif
