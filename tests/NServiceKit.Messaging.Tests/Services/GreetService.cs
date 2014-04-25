using System.Runtime.Serialization;
using NServiceKit.ServiceInterface;

namespace NServiceKit.Messaging.Tests.Services
{
	[DataContract]
	public class Greet
	{
		[DataMember]
		public string Name { get; set; }
	}

	[DataContract]
	public class GreetResponse
	{
		[DataMember]
		public string Result { get; set; }
	}

	public class GreetService : ServiceInterface.Service
	{
		public int TimesCalled { get; set; }
		public string Result { get; set; }

		public object Any(Greet request)
		{
			this.TimesCalled++;

			Result = "Hello, " + request.Name;
			return new GreetResponse { Result = Result };
		}
	}

}