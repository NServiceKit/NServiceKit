using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceInterface
{
    public interface IServiceBase : IResolver
    {
        IResolver GetResolver();

        /// <summary>
        /// Resolve an alternate Web Service from NServiceKit's IOC container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T ResolveService<T>();

        IRequestContext RequestContext { get; }
    }
}