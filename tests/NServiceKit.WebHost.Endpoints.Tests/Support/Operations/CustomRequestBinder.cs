using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
    /// <summary>A custom request binder.</summary>
	[Route("/customrequestbinder")]
	public class CustomRequestBinder
	{
        /// <summary>Gets or sets a value indicating whether this object is from binder.</summary>
        ///
        /// <value>true if this object is from binder, false if not.</value>
		public bool IsFromBinder { get; set; }
	}

    /// <summary>A custom request binder response.</summary>
	public class CustomRequestBinderResponse
	{
        /// <summary>Gets or sets a value indicating whether from binder.</summary>
        ///
        /// <value>true if from binder, false if not.</value>
		public bool FromBinder { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A custom request binder service.</summary>
    public class CustomRequestBinderService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(CustomRequestBinder request)
		{
			return new CustomRequestBinderResponse { FromBinder = request.IsFromBinder };
		}
	}
}