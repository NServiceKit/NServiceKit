using System;
using System.Runtime.Serialization;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>Exception for signalling request binding errors.</summary>
    public class RequestBindingException : SerializationException
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.RequestBindingException class.</summary>
        ///
        /// <param name="message">       The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RequestBindingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
