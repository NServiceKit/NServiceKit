using System;
using System.Diagnostics;
using NUnit.Framework;
using NServiceKit.Configuration;
using NServiceKit.ServiceHost.Tests.Support;

namespace NServiceKit.ServiceHost.Tests
{
	[Ignore("Perf Test Only")]
	[TestFixture]
	public class PerfTests
	{
		private const int Times = 100000;
		private ServiceController serviceController;

		[SetUp]
		public void OnBeforeEachTest()
		{
			serviceController = new ServiceController(null);
		}

		[Test]
		public void RunAll()
		{
			With_Native();
			With_Expressions();
			With_CustomFunc();
			With_TypeFactory();
			With_TypedArguments();
		}


		[Test]
		public void With_Native()
		{
			var request = new BasicRequest();

			Console.WriteLine("Native(): {0}", Measure(() => new BasicService().Any(request), Times));
		}

		[Test]
		public void With_NServiceKitFunq()
		{
			serviceController.Register(typeof(BasicRequest), typeof(BasicService));
			var request = new BasicRequest();

			Console.WriteLine("With_TypedArguments(): {0}", Measure(() => serviceController.Execute(request), Times));
		}

		[Test]
		public void With_TypedArguments()
		{
            serviceController.Register(typeof(BasicRequest), typeof(BasicService));
			var request = new BasicRequest();

			Console.WriteLine("With_TypedArguments(): {0}", Measure(() => serviceController.Execute(request), Times));
		}

		[Test]
		public void With_Expressions()
		{
			var requestType = typeof(BasicRequest);

			serviceController.Register(requestType, typeof(BasicService));
			var request = new BasicRequest();

			Console.WriteLine("With_Expressions(): {0}", Measure(() => serviceController.Execute(request), Times));
		}

		[Test]
		public void With_CustomFunc()
		{
			var requestType = typeof(BasicRequest);

			serviceController.Register(requestType, typeof(BasicService), type => new BasicService());

			var request = new BasicRequest();

			Console.WriteLine("With_CustomFunc(): {0}", Measure(() => serviceController.Execute(request), Times));
		}

		public class BasicServiceTypeFactory : ITypeFactory
		{
			public object CreateInstance(Type type)
			{
				return new BasicService();
			}
		}

		[Test]
		public void With_TypeFactory()
		{
			var requestType = typeof(BasicRequest);
			serviceController.RegisterGServiceExecutor(requestType, typeof(BasicService), new BasicServiceTypeFactory());

			var request = new BasicRequest();

			Console.WriteLine("With_TypeFactory(): {0}", Measure(() => serviceController.Execute(request), Times));
		}


		private static long Measure(Action action, int iterations)
		{
			GC.Collect();
			var watch = Stopwatch.StartNew();

			for (int i = 0; i < iterations; i++)
			{
				action();
			}

			return watch.ElapsedTicks;
		}
	}
}
