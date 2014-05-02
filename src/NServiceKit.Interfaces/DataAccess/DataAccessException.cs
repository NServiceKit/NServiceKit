using System;
using System.Runtime.Serialization;

namespace NServiceKit.DataAccess
{
    /// <summary>Exception for signalling data access errors.</summary>
	public class DataAccessException : Exception
	{
        /// <summary>Initializes a new instance of the NServiceKit.DataAccess.DataAccessException class.</summary>
		public DataAccessException()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.DataAccess.DataAccessException class.</summary>
        ///
        /// <param name="message">The message.</param>
		public DataAccessException(string message) 
			: base(message)
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.DataAccess.DataAccessException class.</summary>
        ///
        /// <param name="message">       The message.</param>
        /// <param name="innerException">The inner exception.</param>
		public DataAccessException(string message, Exception innerException) 
			: base(message, innerException)
		{
		}

#if !SILVERLIGHT && !MONOTOUCH && !XBOX

        /// <summary>Initializes a new instance of the NServiceKit.DataAccess.DataAccessException class.</summary>
        ///
        /// <param name="info">   The information.</param>
        /// <param name="context">The context.</param>
		protected DataAccessException(SerializationInfo info, StreamingContext context) 
			: base(info, context)
		{
		}
#endif

	}
}