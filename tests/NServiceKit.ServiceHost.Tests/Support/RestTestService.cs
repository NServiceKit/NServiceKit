using System;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceHost.Tests.Support
{
	[DataContract]
	public class RestTest { }

	[DataContract]
	public class RestTestResponse
	{
		[DataMember]
		public string MethodName { get; set; }
	}

	public class RestTestService : ServiceInterface.Service
	{
        public RestTestResponse Any(RestTest request)
		{
			return new RestTestResponse { MethodName = "Execute" };
		}

		public object Get(RestTest request)
		{
			return new RestTestResponse { MethodName = "Get" };
		}

		public object Put(RestTest request)
		{
			return new RestTestResponse { MethodName = "Put" };
		}

		public object Post(RestTest request)
		{
			return new RestTestResponse { MethodName = "Post" };
		}

		public object Delete(RestTest request)
		{
			return new RestTestResponse { MethodName = "Delete" };
		}
	}
}