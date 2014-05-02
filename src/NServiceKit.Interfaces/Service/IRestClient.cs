using System.IO;
using System.Net;
using NServiceKit.ServiceHost;

namespace NServiceKit.Service
{
    /// <summary>Interface for rest client.</summary>
	public interface IRestClient 
	{
        /// <summary>Gets.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
	    TResponse Get<TResponse>(IReturn<TResponse> request);

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        void Get(IReturnVoid request);

        /// <summary>Gets.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        ///
        /// <returns>A TResponse.</returns>
        TResponse Get<TResponse>(string relativeOrAbsoluteUrl);

        /// <summary>Deletes the given relativeOrAbsoluteUrl.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
	    TResponse Delete<TResponse>(IReturn<TResponse> request);

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        void Delete(IReturnVoid request);

        /// <summary>Deletes the given relativeOrAbsoluteUrl.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        ///
        /// <returns>A TResponse.</returns>
        TResponse Delete<TResponse>(string relativeOrAbsoluteUrl);

        /// <summary>Post this message.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
	    TResponse Post<TResponse>(IReturn<TResponse> request);

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        void Post(IReturnVoid request);

        /// <summary>Post this message.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        TResponse Post<TResponse>(string relativeOrAbsoluteUrl, object request);

        /// <summary>Puts.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
	    TResponse Put<TResponse>(IReturn<TResponse> request);

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request.</param>
	    void Put(IReturnVoid request);

        /// <summary>Puts.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        TResponse Put<TResponse>(string relativeOrAbsoluteUrl, object request);

        /// <summary>Patches.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
	    TResponse Patch<TResponse>(IReturn<TResponse> request);

        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
	    void Patch(IReturnVoid request);

        /// <summary>Patches.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
		TResponse Patch<TResponse>(string relativeOrAbsoluteUrl, object request);

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

        /// <summary>Custom method.</summary>
        ///
        /// <param name="httpVerb">The HTTP verb.</param>
        /// <param name="request"> The request.</param>
	    void CustomMethod(string httpVerb, IReturnVoid request);

        /// <summary>Custom method.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="httpVerb">The HTTP verb.</param>
        /// <param name="request"> The request.</param>
        ///
        /// <returns>A TResponse.</returns>
	    TResponse CustomMethod<TResponse>(string httpVerb, IReturn<TResponse> request);

        /// <summary>Heads.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpWebResponse.</returns>
        HttpWebResponse Head(IReturn request);

        /// <summary>Heads.</summary>
        ///
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        ///
        /// <returns>A HttpWebResponse.</returns>
        HttpWebResponse Head(string relativeOrAbsoluteUrl);
	}
}