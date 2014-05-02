namespace NServiceKit.Service
{
    /// <summary>Interface for response status.</summary>
	public interface IResponseStatus
	{
        /// <summary>Gets or sets the error code.</summary>
        ///
        /// <value>The error code.</value>
		string ErrorCode { get; set; }

        /// <summary>Gets or sets a message describing the error.</summary>
        ///
        /// <value>A message describing the error.</value>
		string ErrorMessage { get; set; }

        /// <summary>Gets or sets the stack trace.</summary>
        ///
        /// <value>The stack trace.</value>
		string StackTrace { get; set; }

        /// <summary>Gets a value indicating whether this object is success.</summary>
        ///
        /// <value>true if this object is success, false if not.</value>
		bool IsSuccess { get; }
	}
}