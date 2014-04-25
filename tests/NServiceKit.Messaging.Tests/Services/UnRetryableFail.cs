using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;

namespace NServiceKit.Messaging.Tests.Services
{
	[DataContract]
	public class UnRetryableFail
	{
		[DataMember]
		public string Name { get; set; }
	}

	[DataContract]
	public class UnRetryableFailResponse
	{
		[DataMember]
		public string Result { get; set; }
	}

    public class UnRetryableFailService : ServiceInterface.Service
	{
		public int TimesCalled { get; set; }
		public string Result { get; set; }

		public object Any(UnRetryableFail request)
		{
			this.TimesCalled++;

			throw new UnRetryableMessagingException(
				"This request should not get retried",
				new NotSupportedException("This service always fails"));
		}

        public object ExecuteAsync(IMessage<UnRetryableFail> request)
        {
            return Any(request.GetBody());
        }
	}

}