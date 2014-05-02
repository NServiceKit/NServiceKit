using System;
using System.Runtime.Serialization;

namespace NServiceKit.Messaging
{
	/// <summary>
	/// Base Exception for all NServiceKit.Messaging exceptions
	/// </summary>
	public class MessagingException
		: Exception
	{
        /// <summary>Initializes a new instance of the NServiceKit.Messaging.MessagingException class.</summary>
		public MessagingException()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.MessagingException class.</summary>
        ///
        /// <param name="message">The message.</param>
		public MessagingException(string message)
			: base(message)
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.MessagingException class.</summary>
        ///
        /// <param name="message">       The message.</param>
        /// <param name="innerException">The inner exception.</param>
		public MessagingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

#if !SILVERLIGHT && !MONOTOUCH && !XBOX

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.MessagingException class.</summary>
        ///
        /// <param name="info">   The information.</param>
        /// <param name="context">The context.</param>
		protected MessagingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
#endif

        /// <summary>Converts this object to a message error.</summary>
        ///
        /// <returns>This object as a MessageError.</returns>
        public virtual MessageError ToMessageError()
		{
			return new MessageError {
				ErrorCode = GetType().Name,
				Message = this.Message,
				StackTrace = this.ToString(), //Also includes inner exception
			};
		}
	}
}
