using Funq;
using NServiceKit.WebHost.Endpoints.Tests.Support.Services;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Host
{
    /// <summary>A test application host HTTP listener.</summary>
	public class TestAppHostHttpListener
		: AppHostHttpListenerBase
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Host.TestAppHostHttpListener class.</summary>
		public TestAppHostHttpListener()
			: base("Example Service", typeof(TestService).Assembly)
		{
			Instance = null;
		}

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
		public override void Configure(Container container)
		{
			container.Register<IFoo>(c => new Foo());
		}
	}
}