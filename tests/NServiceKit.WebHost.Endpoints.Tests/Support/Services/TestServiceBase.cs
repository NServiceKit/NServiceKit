using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
	public abstract class TestServiceBase<TRequest> : ServiceInterface.Service
	{
		protected abstract object Run(TRequest request);

		public object Any(TRequest request)
		{
			return Run(request);
		}
	}
}