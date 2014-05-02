#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using NServiceKit.Service;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.Common.Web
{
    /// <summary>Encapsulates the result of a http.</summary>
    public class HttpResult
        : IHttpResult, IStreamWriter, IPartialWriter
    {
        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        public HttpResult()
            : this((object)null, null)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        ///
        /// <param name="response">The response.</param>
        public HttpResult(object response)
            : this(response, null)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        ///
        /// <param name="response">   The response.</param>
        /// <param name="contentType">The type of the content.</param>
        public HttpResult(object response, string contentType)
            : this(response, contentType, HttpStatusCode.OK)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        ///
        /// <param name="statusCode">       The status code.</param>
        /// <param name="statusDescription">Information describing the status.</param>
        public HttpResult(HttpStatusCode statusCode, string statusDescription)
            : this()
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        ///
        /// <param name="response">  The response.</param>
        /// <param name="statusCode">The status code.</param>
        public HttpResult(object response, HttpStatusCode statusCode)
            : this(response, null, statusCode) { }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        ///
        /// <param name="response">   The response.</param>
        /// <param name="contentType">The type of the content.</param>
        /// <param name="statusCode"> The status code.</param>
        public HttpResult(object response, string contentType, HttpStatusCode statusCode)
        {
            this.Headers = new Dictionary<string, string>();
            this.ResponseFilter = HttpResponseFilter.Instance;

            this.Response = response;
            this.ContentType = contentType;
            this.StatusCode = statusCode;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        ///
        /// <param name="fileResponse">The file response.</param>
        /// <param name="asAttachment">true to as attachment.</param>
        public HttpResult(FileInfo fileResponse, bool asAttachment)
            : this(fileResponse, MimeTypes.GetMimeType(fileResponse.Name), asAttachment) { }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        ///
        /// <param name="fileResponse">The file response.</param>
        /// <param name="contentType"> The type of the content.</param>
        /// <param name="asAttachment">true to as attachment.</param>
        public HttpResult(FileInfo fileResponse, string contentType=null, bool asAttachment=false)
            : this(null, contentType ?? MimeTypes.GetMimeType(fileResponse.Name), HttpStatusCode.OK)
        {
            this.FileInfo = fileResponse;
            this.AllowsPartialResponse = true;

            if (!asAttachment) return;

            var headerValue =
                "attachment; " +
                "filename=\"" + fileResponse.Name + "\"; " +
                "size=" + fileResponse.Length + "; " +
                "creation-date=" + fileResponse.CreationTimeUtc.ToString("R").Replace(",", "") + "; " +
                "modification-date=" + fileResponse.LastWriteTimeUtc.ToString("R").Replace(",", "") + "; " +
                "read-date=" + fileResponse.LastAccessTimeUtc.ToString("R").Replace(",", "");

            this.Headers = new Dictionary<string, string> {
                { HttpHeaders.ContentDisposition, headerValue },
            };
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        ///
        /// <param name="responseStream">The response stream.</param>
        /// <param name="contentType">   The type of the content.</param>
        public HttpResult(Stream responseStream, string contentType)
            : this(null, contentType, HttpStatusCode.OK)
        {
            this.AllowsPartialResponse = true;
            this.ResponseStream = responseStream;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        ///
        /// <param name="responseText">The response text.</param>
        /// <param name="contentType"> The type of the content.</param>
        public HttpResult(string responseText, string contentType)
            : this(null, contentType, HttpStatusCode.OK)
        {
            this.AllowsPartialResponse = true;
            this.ResponseText = responseText;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResult class.</summary>
        ///
        /// <param name="responseBytes">The response in bytes.</param>
        /// <param name="contentType">  The type of the content.</param>
        public HttpResult(byte[] responseBytes, string contentType)
            : this(null, contentType, HttpStatusCode.OK)
        {
            this.AllowsPartialResponse = true;
            this.ResponseStream = new MemoryStream(responseBytes);
        }

        /// <summary>Gets the response text.</summary>
        ///
        /// <value>The response text.</value>
        public string ResponseText { get; private set; }

        /// <summary>Gets the response stream.</summary>
        ///
        /// <value>The response stream.</value>
        public Stream ResponseStream { get; private set; }

        /// <summary>Gets information describing the file.</summary>
        ///
        /// <value>Information describing the file.</value>
        public FileInfo FileInfo { get; private set; }

        /// <summary>The HTTP Response ContentType.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>Additional HTTP Headers.</summary>
        ///
        /// <value>The headers.</value>
        public Dictionary<string, string> Headers { get; private set; }

        private bool allowsPartialResponse;

        /// <summary>Gets or sets a value indicating whether we allows partial response.</summary>
        ///
        /// <value>true if allows partial response, false if not.</value>
        public bool AllowsPartialResponse
        {
            set
            {
                allowsPartialResponse = value;
                if (allowsPartialResponse)
                    this.Headers.Add(HttpHeaders.AcceptRanges, "bytes");
                else
                    this.Headers.Remove(HttpHeaders.AcceptRanges);
            }
            get { return allowsPartialResponse; }
        }

        /// <summary>Sets the Date/Time of the last modified.</summary>
        ///
        /// <value>The last modified.</value>
        public DateTime LastModified
        {
            set
            {
                this.Headers[HttpHeaders.LastModified] = value.ToUniversalTime().ToString("r");
            }
        }

        /// <summary>Sets the location.</summary>
        ///
        /// <value>The location.</value>
        public string Location
        {
            set
            {
                if (StatusCode == HttpStatusCode.OK)
                    StatusCode = HttpStatusCode.Redirect;

                this.Headers[HttpHeaders.Location] = value;
            }
        }

        /// <summary>Sets permanent cookie.</summary>
        ///
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        public void SetPermanentCookie(string name, string value)
        {
            SetCookie(name, value, DateTime.UtcNow.AddYears(20), null);
        }

        /// <summary>Sets permanent cookie.</summary>
        ///
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="path"> Full pathname of the file.</param>
        public void SetPermanentCookie(string name, string value, string path)
        {
            SetCookie(name, value, DateTime.UtcNow.AddYears(20), path);
        }

        /// <summary>Sets session cookie.</summary>
        ///
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        public void SetSessionCookie(string name, string value)
        {
            SetSessionCookie(name, value, null);
        }

        /// <summary>Sets session cookie.</summary>
        ///
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="path"> Full pathname of the file.</param>
        public void SetSessionCookie(string name, string value, string path)
        {
            path = path ?? "/";
            this.Headers[HttpHeaders.SetCookie] = string.Format("{0}={1};path=" + path, name, value);
        }

        /// <summary>Sets a cookie.</summary>
        ///
        /// <param name="name">     The name.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        /// <param name="path">     Full pathname of the file.</param>
        public void SetCookie(string name, string value, TimeSpan expiresIn, string path)
        {
            var expiresAt = DateTime.UtcNow.Add(expiresIn);
            SetCookie(name, value, expiresAt, path);
        }

        /// <summary>Sets a cookie.</summary>
        ///
        /// <param name="name">     The name.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresAt">The expires at Date/Time.</param>
        /// <param name="path">     Full pathname of the file.</param>
        public void SetCookie(string name, string value, DateTime expiresAt, string path)
        {
            path = path ?? "/";
            var cookie = string.Format("{0}={1};expires={2};path={3}", name, value, expiresAt.ToString("R"), path);
            this.Headers[HttpHeaders.SetCookie] = cookie;
        }

        /// <summary>Deletes the cookie described by name.</summary>
        ///
        /// <param name="name">The name.</param>
        public void DeleteCookie(string name)
        {
            var cookie = string.Format("{0}=;expires={1};path=/", name, DateTime.UtcNow.AddDays(-1).ToString("R"));
            this.Headers[HttpHeaders.SetCookie] = cookie;
        }

        /// <summary>Gets options for controlling the operation.</summary>
        ///
        /// <value>The options.</value>
        public IDictionary<string, string> Options
        {
            get { return this.Headers; }
        }

        /// <summary>The HTTP Response Status.</summary>
        ///
        /// <value>The status.</value>
        public int Status { get; set; }

        /// <summary>The HTTP Response Status Code.</summary>
        ///
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode)Status; }
            set { Status = (int)value; }
        }

        /// <summary>The HTTP Status Description.</summary>
        ///
        /// <value>Information describing the status.</value>
        public string StatusDescription { get; set; }

        /// <summary>Response DTO.</summary>
        ///
        /// <value>The response.</value>
        public object Response { get; set; }

        /// <summary>if not provided, get's injected by NServiceKit.</summary>
        ///
        /// <value>The response filter.</value>
        public IContentTypeWriter ResponseFilter { get; set; }

        /// <summary>Holds the request call context.</summary>
        ///
        /// <value>The request context.</value>
        public IRequestContext RequestContext { get; set; }

        /// <summary>Gets or sets the view.</summary>
        ///
        /// <value>The view.</value>
        public string View { get; set; }

        /// <summary>Gets or sets the template.</summary>
        ///
        /// <value>The template.</value>
        public string Template { get; set; }

        /// <summary>Writes to.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="responseStream">The response stream.</param>
        public void WriteTo(Stream responseStream)
        {
            if (this.FileInfo != null)
            {
                using (var fs = this.FileInfo.OpenRead())
                {
                    fs.WriteTo(responseStream);
                    responseStream.Flush();
                }
                return;
            }

            if (this.ResponseStream != null)
            {
                this.ResponseStream.WriteTo(responseStream);
                responseStream.Flush();
                DisposeStream();

                return;
            }

            if (this.ResponseText != null)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(this.ResponseText);
                responseStream.Write(bytes, 0, bytes.Length);
                responseStream.Flush();
                return;
            }

            if (this.ResponseFilter == null)
                throw new ArgumentNullException("ResponseFilter");
            if (this.RequestContext == null)
                throw new ArgumentNullException("RequestContext");

            var bytesResponse = this.Response as byte[];
            if (bytesResponse != null)
            {
                responseStream.Write(bytesResponse, 0, bytesResponse.Length);
                return;
            }

            if (View != null)
                RequestContext.SetItem("View", View);
            if (Template != null)
                RequestContext.SetItem("Template", Template);

            RequestContext.SetItem("HttpResult", this);

            ResponseFilter.SerializeToStream(this.RequestContext, this.Response, responseStream);
        }

        /// <summary>Whether this HttpResult allows Partial Response.</summary>
        ///
        /// <value>true if this object is partial request, false if not.</value>
        public bool IsPartialRequest
        {
            get { return AllowsPartialResponse && RequestContext.GetHeader(HttpHeaders.Range) != null && GetContentLength() != null; }
        }

        /// <summary>Write a partial content result.</summary>
        ///
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        ///
        /// <param name="response">The response.</param>
        public void WritePartialTo(IHttpResponse response)
        {
            var contentLength = GetContentLength().GetValueOrDefault(int.MaxValue); //Safe as guarded by IsPartialRequest
            var rangeHeader = RequestContext.GetHeader(HttpHeaders.Range);

            long rangeStart, rangeEnd;
            rangeHeader.ExtractHttpRanges(contentLength, out rangeStart, out rangeEnd);

            if (rangeEnd > contentLength - 1)
                rangeEnd = contentLength - 1;

            response.AddHttpRangeResponseHeaders(rangeStart, rangeEnd, contentLength);

            var outputStream = response.OutputStream;
            if (FileInfo != null)
            {
                using (var fs = FileInfo.OpenRead())
                {
                    fs.WritePartialTo(outputStream, rangeStart, rangeEnd);
                }
            }
            else if (ResponseStream != null)
            {
                ResponseStream.WritePartialTo(outputStream, rangeStart, rangeEnd);
                DisposeStream();
            }
            else if (ResponseText != null)
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(ResponseText)))
                {
                    ms.WritePartialTo(outputStream, rangeStart, rangeEnd);
                }
            }
            else
                throw new InvalidOperationException("Neither file, stream nor text were set when attempting to write to the Response Stream.");
        }

        /// <summary>Gets content length.</summary>
        ///
        /// <returns>The content length.</returns>
        public long? GetContentLength()
        {
            if (FileInfo != null)
                return FileInfo.Length;
            if (ResponseStream != null)
                return ResponseStream.Length;
            if (ResponseText != null)
                return ResponseText.Length;
            return null;
        }

        /// <summary>Status 201 created.</summary>
        ///
        /// <param name="response">      The response.</param>
        /// <param name="newLocationUri">URI of the new location.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public static HttpResult Status201Created(object response, string newLocationUri)
        {
            return new HttpResult(response)
            {
                StatusCode = HttpStatusCode.Created,
                Headers =
                {
                    { HttpHeaders.Location, newLocationUri },
                }
            };
        }

        /// <summary>Redirects.</summary>
        ///
        /// <param name="newLocationUri">URI of the new location.</param>
        /// <param name="redirectStatus">The redirect status.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public static HttpResult Redirect(string newLocationUri, HttpStatusCode redirectStatus = HttpStatusCode.Found)
        {
            return new HttpResult
            {
                StatusCode = redirectStatus,
                Headers =
                {
                    { HttpHeaders.Location, newLocationUri },
                }
            };
        }

        /// <summary>Dispose stream.</summary>
        public void DisposeStream()
        {
            try
            {
                if (ResponseStream != null)
                {
                    this.ResponseStream.Dispose();
                }
            }
            catch { /*ignore*/ }
        }
    }
}
#endif