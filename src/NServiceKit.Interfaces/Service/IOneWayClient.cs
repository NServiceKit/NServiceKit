namespace NServiceKit.Service
{
    /// <summary>
    /// Clients for sending one-way messages.
    /// </summary>
	public interface IOneWayClient
	{
        /// <summary>
        /// Sends the request.
        /// </summary>
        /// <param name="request"></param>
        void SendOneWay(object request);

        /// <summary>
        /// Sends the request to the specified URL.
        /// </summary>
        /// <param name="relativeOrAbsoluteUrl"></param>
        /// <param name="request"></param>
        void SendOneWay(string relativeOrAbsoluteUrl, object request);
    }
}