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
		public MessagingException()
		{
		}

		public MessagingException(string message)
			: base(message)
		{
		}

		public MessagingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
		protected MessagingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
#endif
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
