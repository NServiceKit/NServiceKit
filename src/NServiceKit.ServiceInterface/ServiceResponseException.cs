using System;
using NServiceKit.Common.Extensions;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.ServiceInterface
{
    /// <summary>Exception for signalling service response errors.</summary>
    public class ServiceResponseException
        : Exception
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceResponseException class.</summary>
        public ServiceResponseException()
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceResponseException class.</summary>
        ///
        /// <param name="message">The message.</param>
        public ServiceResponseException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceResponseException class.</summary>
        ///
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        public ServiceResponseException(string errorCode, string errorMessage)
            : base(GetErrorMessage(errorCode, errorMessage))
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceResponseException class.</summary>
        ///
        /// <param name="errorCode">       The error code.</param>
        /// <param name="errorMessage">    Message describing the error.</param>
        /// <param name="NServiceKitTrace">The n service kit trace.</param>
        public ServiceResponseException(string errorCode, string errorMessage, string NServiceKitTrace)
            : base(errorMessage)
        {
            this.ErrorCode = errorCode;
            this.NServiceKitTrace = NServiceKitTrace;
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.ServiceResponseException class.</summary>
        ///
        /// <param name="responseStatus">The response status.</param>
        public ServiceResponseException(ResponseStatus responseStatus)
            : base(GetErrorMessage(responseStatus.ErrorCode, responseStatus.Message))
        {
            this.ErrorCode = responseStatus.ErrorCode;
        }

        private static string GetErrorMessage(string errorCode, string errorMessage)
        {
            return errorMessage ?? (errorCode != null ? errorCode.ToEnglish() : null); 
        }

        /// <summary>Gets or sets the error code.</summary>
        ///
        /// <value>The error code.</value>
        public string ErrorCode { get; set; }

        /// <summary>Gets or sets the service kit trace.</summary>
        ///
        /// <value>The n service kit trace.</value>
        public string NServiceKitTrace { get; set; }
    }
}