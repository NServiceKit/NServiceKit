using System;
using System.Diagnostics;
using NUnit.Framework;
using NServiceKit.Configuration;
using NServiceKit.ServiceHost.Tests.Support;

namespace NServiceKit.ServiceHost.Tests
{
    /// <summary>A performance tests.</summary>
	[Ignore("Perf Test Only")]
	[TestFixture]
	public class PerfTests
	{
		private const int Times = 100000;
		private ServiceController serviceController;

        /// <summary>Executes the before each test action.</summary>
		[SetUp]
		public void OnBeforeEachTest()
		{
			serviceController = new ServiceController(null);
		}

        /// <summary>Executes all operation.</summary>
		[Test]
		public void RunAll()
		{
			With_Native();
			With_Expressions();
			With_CustomFunc();
			With_TypeFactory();
			With_TypedArguments();
		}


        /// <summary>With native.</summary>
		[Test]
		public void With_Native()
		{
			var request = new BasicRequest();

			Console.WriteLine("Native(): {0}", Measure(() => new BasicService().Any(request), Times));
		}

        /// <summary>With n service kit funq.</summary>
		[Test]
		public void With_NServiceKitFunq()
		{
			serviceController.Register(typeof(BasicRequest), typeof(BasicService));
			var request = new BasicRequest();

			Console.WriteLine("With_TypedArguments(): {0}", Measure(() => serviceController.Execute(request), Times));
		}

        /// <summary>With typed arguments.</summary>
		[Test]
		public void With_TypedArguments()
		{
            serviceController.Register(typeof(BasicRequest), typeof(BasicService));
			var request = new BasicRequest();

			Console.WriteLine("With_TypedArguments(): {0}", Measure(() => serviceController.Execute(request), Times));
		}

        /// <summary>With expressions.</summary>
		[Test]
		public void With_Expressions()
		{
			var requestType = typeof(BasicRequest);

			serviceController.Register(requestType, typeof(BasicService));
			var request = new BasicRequest();

			Console.WriteLine("With_Expressions(): {0}", Measure(() => serviceController.Execute(request), Times));
		}

        /// <summary>With custom function.</summary>
		[Test]
		public void With_CustomFunc()
		{
			var requestType = typeof(BasicRequest);

			serviceController.Register(requestType, typeof(BasicService), type => new BasicService());

			var request = new BasicRequest();

			Console.WriteLine("With_CustomFunc(): {0}", Measure(() => serviceController.Execute(request), Times));
		}

        /// <summary>A basic service type factory.</summary>
		public class BasicServiceTypeFactory : ITypeFactory
		{
            /// <summary>Creates an instance.</summary>
            ///
            /// <param name="type">The type.</param>
            ///
            /// <returns>The new instance.</returns>
			public object CreateInstance(Type type)
			{
				return new BasicService();
			}
		}

        /// <summary>With type factory.</summary>
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
