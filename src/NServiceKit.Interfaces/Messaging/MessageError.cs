namespace NServiceKit.Messaging
{
	/// <summary>
	/// An Error Message Type that can be easily serialized
	/// </summary>
	public class MessageError
	{
        /// <summary>Gets or sets the error code.</summary>
        ///
        /// <value>The error code.</value>
		public string ErrorCode { get; set; }

        /// <summary>Gets or sets the message.</summary>
        ///
        /// <value>The message.</value>
		public string Message { get; set; }

        /// <summary>Gets or sets the stack trace.</summary>
        ///
        /// <value>The stack trace.</value>
		public string StackTrace { get; set; }
	}
}