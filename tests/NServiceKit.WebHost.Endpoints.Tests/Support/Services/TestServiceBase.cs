using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A test service base.</summary>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
	public abstract class TestServiceBase<TRequest> : ServiceInterface.Service
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected abstract object Run(TRequest request);

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(TRequest request)
		{
			return Run(request);
		}
	}
}