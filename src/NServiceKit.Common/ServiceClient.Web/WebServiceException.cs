using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using NServiceKit.Common.Utils;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>Exception for signalling web service errors.</summary>
#if !NETFX_CORE && !WINDOWS_PHONE && !SILVERLIGHT
    [Serializable]
#endif
    public class WebServiceException
        : Exception
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.WebServiceException class.</summary>
        public WebServiceException() { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.WebServiceException class.</summary>
        ///
        /// <param name="message">The message.</param>
        public WebServiceException(string message) : base(message) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.WebServiceException class.</summary>
        ///
        /// <param name="message">       The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public WebServiceException(string message, Exception innerException) : base(message, innerException) { }
#if !NETFX_CORE && !WINDOWS_PHONE && !SILVERLIGHT

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.WebServiceException class.</summary>
        ///
        /// <param name="info">   The information.</param>
        /// <param name="context">The context.</param>
        public WebServiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
        public int StatusCode { get; set; }

        /// <summary>Gets or sets information describing the status.</summary>
        ///
        /// <value>Information describing the status.</value>
        public string StatusDescription { get; set; }

        /// <summary>Gets or sets the response dto.</summary>
        ///
        /// <value>The response dto.</value>
        public object ResponseDto { get; set; }

        /// <summary>Gets or sets the response body.</summary>
        ///
        /// <value>The response body.</value>
        public string ResponseBody { get; set; }

        private string errorCode;

        private void ParseResponseDto()
        {
            string responseStatus;
            if (!TryGetResponseStatusFromResponseDto(out responseStatus))
            {
                if (!TryGetResponseStatusFromResponseBody(out responseStatus))
                {
                    errorCode = StatusDescription;
                    return;
                }
            }

            var rsMap = TypeSerializer.DeserializeFromString<Dictionary<string, string>>(responseStatus);
            if (rsMap == null) return;

            rsMap = new Dictionary<string, string>(rsMap, StringExtensions.InvariantComparerIgnoreCase());
            rsMap.TryGetValue("ErrorCode", out errorCode);
            rsMap.TryGetValue("Message", out errorMessage);
            rsMap.TryGetValue("StackTrace", out serverStackTrace);
        }

        private bool TryGetResponseStatusFromResponseDto(out string responseStatus)
        {
            responseStatus = String.Empty;
            try
            {
                if (ResponseDto == null)
                    return false;
                var jsv = TypeSerializer.SerializeToString(ResponseDto);
                var map = TypeSerializer.DeserializeFromString<Dictionary<string, string>>(jsv);
                map = new Dictionary<string, string>(map, StringExtensions.InvariantComparerIgnoreCase());

                return map.TryGetValue("ResponseStatus", out responseStatus);
            }
            catch
            {
                return false;
            }
        }

        private bool TryGetResponseStatusFromResponseBody(out string responseStatus)
        {
            responseStatus = String.Empty;
            try
            {
                if (String.IsNullOrEmpty(ResponseBody)) return false;
                var map = TypeSerializer.DeserializeFromString<Dictionary<string, string>>(ResponseBody);
                map = new Dictionary<string, string>(map, StringExtensions.InvariantComparerIgnoreCase());
                return map.TryGetValue("ResponseStatus", out responseStatus);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Gets the error code.</summary>
        ///
        /// <value>The error code.</value>
        public string ErrorCode
        {
            get
            {
                if (errorCode == null)
                {
                    ParseResponseDto();
                }
                return errorCode;
            }
        }

        private string errorMessage;

        /// <summary>Gets a message describing the error.</summary>
        ///
        /// <value>A message describing the error.</value>
        public string ErrorMessage
        {
            get
            {
                if (errorMessage == null)
                {
                    ParseResponseDto();
                }
                return errorMessage;
            }
        }

        private string serverStackTrace;

        /// <summary>Gets the server stack trace.</summary>
        ///
        /// <value>The server stack trace.</value>
        public string ServerStackTrace
        {
            get
            {
                if (serverStackTrace == null)
                {
                    ParseResponseDto();
                }
                return serverStackTrace;
            }
        }

        /// <summary>Gets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus
        {
            get
            {
                if (this.ResponseDto == null)
                    return null;

                var hasResponseStatus = this.ResponseDto as IHasResponseStatus;
                if (hasResponseStatus != null)
                    return hasResponseStatus.ResponseStatus;

                var propertyInfo = this.ResponseDto.GetType().GetPropertyInfo("ResponseStatus");
                if (propertyInfo == null)
                    return null;

                return ReflectionUtils.GetProperty(this.ResponseDto, propertyInfo) as ResponseStatus;
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
    }
}
