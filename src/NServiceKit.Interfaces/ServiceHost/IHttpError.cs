namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for HTTP error.</summary>
	public interface IHttpError : IHttpResult
	{
        /// <summary>Gets the message.</summary>
        ///
        /// <value>The message.</value>
		string Message { get; }

        /// <summary>Gets the error code.</summary>
        ///
        /// <value>The error code.</value>
		string ErrorCode { get; }
	}
}