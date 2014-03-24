using Funq;
using NServiceKit.WebHost.Endpoints.Tests.Support.Services;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Host
{

	public interface IFoo { }
	public class Foo : IFoo { }

	public class TestAppHost
		: AppHostBase
	{
		public TestAppHost()
			: base("Example Service", typeof(Nested).Assembly)
		{
			Instance = null;
		}

		public override void Configure(Container container)
		{
			container.Register<IFoo>(c => new Foo());
		}
	}
}