using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A test asynchronous.</summary>
	[DataContract]
	public class TestAsync { }

    /// <summary>A test asynchronous response.</summary>
	[DataContract]
	public class TestAsyncResponse
	{
        /// <summary>Gets or sets the foo.</summary>
        ///
        /// <value>The foo.</value>
		[DataMember]
		public IFoo Foo { get; set; }

        /// <summary>Gets or sets a list of times of the executes.</summary>
        ///
        /// <value>A list of times of the executes.</value>
		[DataMember]
		public int ExecuteTimes { get; set; }

        /// <summary>Gets or sets a list of times of the execute asynchronous.</summary>
        ///
        /// <value>A list of times of the execute asynchronous.</value>
		[DataMember]
		public int ExecuteAsyncTimes { get; set; }
	}

    /// <summary>A test asynchronous service.</summary>
	public class TestAsyncService : ServiceInterface.Service
	{
		private readonly IFoo foo;

        /// <summary>Gets a list of times of the executes.</summary>
        ///
        /// <value>A list of times of the executes.</value>
		public static int ExecuteTimes { get; private set; }

        /// <summary>Gets a list of times of the execute asynchronous.</summary>
        ///
        /// <value>A list of times of the execute asynchronous.</value>
		public static int ExecuteAsyncTimes { get; private set; }
		
        /// <summary>Resets the statistics.</summary>
		public static void ResetStats()
		{
			ExecuteTimes = 0;
			ExecuteAsyncTimes = 0;
		}

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.TestAsyncService class.</summary>
        ///
        /// <param name="foo">The foo.</param>
		public TestAsyncService(IFoo foo)
		{
			this.foo = foo;
		}

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TestAsyncResponse.</returns>
        public TestAsyncResponse Any(TestAsync request)
		{
			return new TestAsyncResponse { Foo = this.foo, ExecuteTimes = ++ExecuteTimes };
		}
	}
}