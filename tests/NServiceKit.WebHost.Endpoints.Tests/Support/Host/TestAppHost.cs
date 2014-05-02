using Funq;
using NServiceKit.WebHost.Endpoints.Tests.Support.Services;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Host
{

    /// <summary>Interface for foo.</summary>
	public interface IFoo { }
    /// <summary>A foo.</summary>
	public class Foo : IFoo { }

    /// <summary>A test application host.</summary>
	public class TestAppHost
		: AppHostBase
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Host.TestAppHost class.</summary>
		public TestAppHost()
			: base("Example Service", typeof(Nested).Assembly)
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