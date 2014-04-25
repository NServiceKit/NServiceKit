using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
	[DataContract]
	public class Test { }

	[DataContract]
	public class TestResponse
	{
		[DataMember]
		public IFoo Foo { get; set; }

		[DataMember]
		public int ExecuteTimes { get; set; }

		[DataMember]
		public int ExecuteAsyncTimes { get; set; }
	}

	public class TestService : ServiceInterface.Service
	{
		private readonly IFoo foo;

		public static int ExecuteTimes { get; private set; }
		public static int ExecuteAsyncTimes { get; private set; }

		public static void ResetStats()
		{
			ExecuteTimes = 0;
			ExecuteAsyncTimes = 0;
		}

		public TestService(IFoo foo)
		{
			this.foo = foo;
		}

        public TestResponse Any(Test request)
		{
			return new TestResponse { Foo = this.foo, ExecuteTimes = ++ExecuteTimes };
		}
	}
}