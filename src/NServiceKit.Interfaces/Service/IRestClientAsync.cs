using System;
using NServiceKit.ServiceHost;

namespace NServiceKit.Service
{
    /// <summary>
    /// A client that makes asynchronous REST requests.
    /// </summary>
	public interface IRestClientAsync : IDisposable
	{
        /// <summary>
        /// Set the credentials to be used when making requests.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
		void SetCredentials(string userName, string password);

        /// <summary>
        /// Sends the request as an HTTP GET asynchronously.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        void GetAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);

        /// <summary>
        /// Makes an HTTP GET to the specified URL asynchronously.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="relativeOrAbsoluteUrl"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        void GetAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);
        
        /// <summary>
        /// Makes an HTTP DELETE to the specified URL asynchronously.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="relativeOrAbsoluteUrl"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        void DeleteAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);

        /// <summary>
        /// Sends the request as an HTTP DELETE asynchronously.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        void DeleteAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);

        /// <summary>
        /// Sends the request as an HTTP POST asynchronously.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        void PostAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);

        /// <summary>
        /// Makes an HTTP POST to the specified URL asynchronously.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="relativeOrAbsoluteUrl"></param>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
		void PostAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);

        /// <summary>
        /// Sends the request as an HTTP PUT asynchronously.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        void PutAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);

        /// <summary>
        /// Makes an HTTP PUT to the specified URL asynchronously.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="relativeOrAbsoluteUrl"></param>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        void PutAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);

        /// <summary>
        /// Sends the specified request with the specified verb asynchronously.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpVerb"></param>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        void CustomMethodAsync<TResponse>(string httpVerb, IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);

        /// <summary>
        /// Cancels pending async operations.
        /// </summary>
	    void CancelAsync();
	}

}