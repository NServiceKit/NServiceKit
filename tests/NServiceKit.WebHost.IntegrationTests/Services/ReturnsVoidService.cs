using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>The returns void.</summary>
	[Route("/returnsvoid")]
	public class ReturnsVoid : IReturnVoid
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }
	}

    /// <summary>The returns void service.</summary>
    public class ReturnsVoidService : IService
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        public void Any(ReturnsVoid request) {}
	}

}