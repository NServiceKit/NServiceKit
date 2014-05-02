using System;

namespace NServiceKit.Service
{
    /// <summary>
    /// 
    /// </summary>
	public interface IServiceClientAsync : IRestClientAsync
	{
        /// <summary>Sends the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
		void SendAsync<TResponse>(object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);
	}
}