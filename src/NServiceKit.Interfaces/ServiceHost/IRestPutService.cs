using System;

namespace NServiceKit.ServiceHost
{
	/// <summary>
	/// If the Service also implements this interface,
	/// IRestPutService.Put() will be used instead of IService.Execute() for 
	/// EndpointAttributes.HttpPut requests
	/// </summary>
	/// <typeparam name="T"></typeparam>
    [Obsolete("Use IService - NServiceKit's New API for future services")]
    public interface IRestPutService<T>
	{
		object Put(T request);
	}
}