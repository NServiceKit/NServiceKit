using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;

namespace NServiceKit.Messaging.Tests.Services
{
	[DataContract]
	public class AlwaysFail
	{
		[DataMember]
		public string Name { get; set; }
	}

	[DataContract]
	public class AlwaysFailResponse
	{
		[DataMember]
		public string Result { get; set; }
	}

	public class AlwaysFailService : ServiceInterface.Service
	{
	    public int TimesCalled { get; set; }
		public string Result { get; set; }

		public object Any(AlwaysFail request)
		{
			this.TimesCalled++;
			throw new NotSupportedException("This service always fails");
		}

	    public object ExecuteAsync(IMessage<AlwaysFail> request)
	    {
	        return Any(request.GetBody());
	    }
	}

}