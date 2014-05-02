namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for resolver.</summary>
	public interface IResolver
	{
		/// <summary>
		/// Resolve a dependency from the AppHost's IOC
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T TryResolve<T>();
	}
}