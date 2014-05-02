using System;
using System.Runtime.Serialization;

namespace NServiceKit.Messaging
{
	/// <summary>
	/// For messaging exceptions that should by-pass the messaging service's configured
	/// retry attempts and store the message straight into the DLQ
	/// </summary>
	public class UnRetryableMessagingException 
		: MessagingException
	{
        /// <summary>Initializes a new instance of the NServiceKit.Messaging.UnRetryableMessagingException class.</summary>
		public UnRetryableMessagingException()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.UnRetryableMessagingException class.</summary>
        ///
        /// <param name="message">The message.</param>
		public UnRetryableMessagingException(string message) : base(message)
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.UnRetryableMessagingException class.</summary>
        ///
        /// <param name="message">       The message.</param>
        /// <param name="innerException">The inner exception.</param>
		public UnRetryableMessagingException(string message, Exception innerException) : base(message, innerException)
		{
		}

#if !SILVERLIGHT && !MONOTOUCH && !XBOX

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.UnRetryableMessagingException class.</summary>
        ///
        /// <param name="info">   The information.</param>
        /// <param name="context">The context.</param>
		protected UnRetryableMessagingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
#endif
    }
}
