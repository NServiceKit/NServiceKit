using System;

namespace NServiceKit.Service
{
    /// <summary>
    /// 
    /// </summary>
	public interface IServiceClientAsync : IRestClientAsync
	{
		void SendAsync<TResponse>(object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);
	}
}