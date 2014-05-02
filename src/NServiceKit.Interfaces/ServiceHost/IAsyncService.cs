namespace NServiceKit.ServiceHost
{
	/// <summary>
	/// If the Service also implements this interface,
	/// IAsyncService.ExecuteAsync() will be used instead of IService.Execute() for 
	/// EndpointAttributes.AsyncOneWay requests
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IAsyncService<T>
	{
        /// <summary>Executes the asynchronous operation.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		object ExecuteAsync(T request);
	}
}