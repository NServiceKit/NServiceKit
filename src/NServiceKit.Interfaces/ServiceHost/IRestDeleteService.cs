using System;

namespace NServiceKit.ServiceHost
{
	/// <summary>
	/// If the Service also implements this interface,
	/// IRestDeleteService.Delete() will be used instead of IService.Execute() for 
	/// EndpointAttributes.HttpDelete requests
	/// </summary>
	/// <typeparam name="T"></typeparam>
    [Obsolete("Use IService - NServiceKit's New API for future services")]
    public interface IRestDeleteService<T>
	{
        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		object Delete(T request);
	}
}