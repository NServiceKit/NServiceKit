using System;
using System.Collections.Generic;
using System.Net;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.Common.Web
{
    /// <summary>A HTTP error.</summary>
    public class HttpError : Exception, IHttpError
    {
        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpError class.</summary>
        public HttpError() : this(null) {}

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpError class.</summary>
        ///
        /// <param name="message">The message.</param>
        public HttpError(string message)
            : this(HttpStatusCode.InternalServerError, message) {}

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpError class.</summary>
        ///
        /// <param name="statusCode">The status code.</param>
        /// <param name="errorCode"> The error code.</param>
        public HttpError(HttpStatusCode statusCode, string errorCode)
            : this(statusCode, errorCode, null) { }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpError class.</summary>
        ///
        /// <param name="statusCode">The status code.</param>
        /// <param name="errorCode"> The error code.</param>
        public HttpError(int statusCode, string errorCode)
            : this(statusCode, errorCode, null) { }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpError class.</summary>
        ///
        /// <param name="responseDto"> The response dto.</param>
        /// <param name="statusCode">  The status code.</param>
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        public HttpError(object responseDto, HttpStatusCode statusCode, string errorCode, string errorMessage)
            : this(statusCode, errorCode, errorMessage)
        {
            this.Response = responseDto;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpError class.</summary>
        ///
        /// <param name="responseDto"> The response dto.</param>
        /// <param name="statusCode">  The status code.</param>
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        public HttpError(object responseDto, int statusCode, string errorCode, string errorMessage)
            : this(statusCode, errorCode, errorMessage)
        {
            this.Response = responseDto;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpError class.</summary>
        ///
        /// <param name="statusCode">  The status code.</param>
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        public HttpError(HttpStatusCode statusCode, string errorCode, string errorMessage)
            : this((int)statusCode, errorCode, errorMessage){}

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpError class.</summary>
        ///
        /// <param name="statusCode">  The status code.</param>
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        public HttpError(int statusCode, string errorCode, string errorMessage)
            : base(errorMessage ?? errorCode)
        {
            this.ErrorCode = errorCode;
            this.Status = statusCode;
            this.Headers = new Dictionary<string, string>();
            this.StatusDescription = errorCode;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpError class.</summary>
        ///
        /// <param name="statusCode">    The status code.</param>
        /// <param name="innerException">The inner exception.</param>
        public HttpError(HttpStatusCode statusCode, Exception innerException)
            : this(innerException.Message, innerException)
        {
            this.StatusCode = statusCode;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpError class.</summary>
        ///
        /// <param name="message">       The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public HttpError(string message, Exception innerException) : base(message, innerException)
        {
            if (innerException != null)
            {
                this.ErrorCode = innerException.GetType().Name;
            }
            this.Headers = new Dictionary<string, string>();			
        }

        /// <summary>Gets or sets the error code.</summary>
        ///
        /// <value>The error code.</value>
        public string ErrorCode { get; set; }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        ///
        /// <value>The headers.</value>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>Gets or sets the status.</summary>
        ///
        /// <value>The status.</value>
        public int Status { get; set; }

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode)Status; }
            set { Status = (int)value; }
        }

        /// <summary>Gets or sets information describing the status.</summary>
        ///
        /// <value>Information describing the status.</value>
        public string StatusDescription { get; set; }

        /// <summary>Gets or sets the response.</summary>
        ///
        /// <value>The response.</value>
        public object Response { get; set; }

        /// <summary>Gets or sets the response filter.</summary>
        ///
        /// <value>The response filter.</value>
        public IContentTypeWriter ResponseFilter { get; set; }

        /// <summary>Gets or sets a context for the request.</summary>
        ///
        /// <value>The request context.</value>
        public IRequestContext RequestContext { get; set; }

        /// <summary>Gets options for controlling the operation.</summary>
        ///
        /// <value>The options.</value>
        public IDictionary<string, string> Options
        {
            get { return this.Headers; }
        }

        /// <summary>Gets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus
        {
            get
            {
                return this.Response.ToResponseStatus();
            }
        }

        /// <summary>Gets field errors.</summary>
        ///
        /// <returns>The field errors.</returns>
        public List<ResponseError> GetFieldErrors()
        {
            var responseStatus = ResponseStatus;
            if (responseStatus != null)
                return responseStatus.Errors ?? new List<ResponseError>();
            
            return new List<ResponseError>();
        }

        /// <summary>Not found.</summary>
        ///
        /// <param name="message">The message.</param>
        ///
        /// <returns>An Exception.</returns>
        public static Exception NotFound(string message)
        {
            return new HttpError(HttpStatusCode.NotFound, message);
        }

        /// <summary>Unauthorized.</summary>
        ///
        /// <param name="message">The message.</param>
        ///
        /// <returns>An Exception.</returns>
        public static Exception Unauthorized(string message)
        {
            return new HttpError(HttpStatusCode.Unauthorized, message);
        }

        /// <summary>Conflicts.</summary>
        ///
        /// <param name="message">The message.</param>
        ///
        /// <returns>An Exception.</returns>
        public static Exception Conflict(string message)
        {
            return new HttpError(HttpStatusCode.Conflict, message);
        }
    }
}