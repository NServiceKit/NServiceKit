using System.IO;
using NServiceKit.ServiceHost;

namespace NServiceKit.Service
{
    /// <summary>Interface for reply client.</summary>
	public interface IReplyClient
	{
		/// <summary>
		/// Sends the specified request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		TResponse Send<TResponse>(object request);

        /// <summary>Sends the specified request.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        TResponse Send<TResponse>(IReturn<TResponse> request);

        /// <summary>Send this message.</summary>
        ///
        /// <param name="request">The request.</param>
	    void Send(IReturnVoid request);

#if !NETFX_CORE

        /// <summary>Posts a file.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="mimeType">             Type of the mime.</param>
        ///
        /// <returns>A TResponse.</returns>
		TResponse PostFile<TResponse>(string relativeOrAbsoluteUrl, FileInfo fileToUpload, string mimeType);
#endif

        /// <summary>Posts a file.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="fileName">             Filename of the file.</param>
        /// <param name="mimeType">             Type of the mime.</param>
        ///
        /// <returns>A TResponse.</returns>
        TResponse PostFile<TResponse>(string relativeOrAbsoluteUrl, Stream fileToUpload, string fileName, string mimeType);

#if !NETFX_CORE

        /// <summary>Posts a file with request.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        TResponse PostFileWithRequest<TResponse>(string relativeOrAbsoluteUrl, FileInfo fileToUpload, object request);
#endif

        /// <summary>Posts a file with request.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="fileName">             Filename of the file.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        TResponse PostFileWithRequest<TResponse>(string relativeOrAbsoluteUrl, Stream fileToUpload, string fileName, object request);
	}
}