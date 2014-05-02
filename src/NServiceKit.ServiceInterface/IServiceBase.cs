using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceInterface
{
    /// <summary>Interface for service base.</summary>
    public interface IServiceBase : IResolver
    {
        /// <summary>Gets the resolver.</summary>
        ///
        /// <returns>The resolver.</returns>
        IResolver GetResolver();

        /// <summary>
        /// Resolve an alternate Web Service from NServiceKit's IOC container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T ResolveService<T>();

        /// <summary>Gets a context for the request.</summary>
        ///
        /// <value>The request context.</value>
        IRequestContext RequestContext { get; }
    }
}