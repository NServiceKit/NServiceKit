namespace NServiceKit.ServiceHost
{
	/// <summary>
	/// If the Service also implements this interface,
	/// IRestPutService.Patch() will be used instead of IService.Execute() for 
	/// EndpointAttributes.HttpPatch requests
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRestPatchService<T>
	{
        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		object Patch(T request);
	}
}