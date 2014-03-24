using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
	[Route("/customrequestbinder")]
	public class CustomRequestBinder
	{
		public bool IsFromBinder { get; set; }
	}

	public class CustomRequestBinderResponse
	{
		public bool FromBinder { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}

    public class CustomRequestBinderService : ServiceInterface.Service
	{
        public object Any(CustomRequestBinder request)
		{
			return new CustomRequestBinderResponse { FromBinder = request.IsFromBinder };
		}
	}
}