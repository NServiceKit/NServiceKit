using System.Runtime.Serialization;
using NServiceKit.Messaging;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
	[DataContract]
	public class MqHostStats
	{
		[DataMember]
		public string Value { get; set; }
	}

	[DataContract]
	public class MqHostStatsResponse
	{
		[DataMember]
		public string Result { get; set; }
	}

	public class MqHostStatsService : ServiceInterface.Service
	{
		public IMessageService MessageService { get; set; }

        public object Any(MqHostStats request)
		{
			return new MqHostStatsResponse { Result = MessageService.GetStatsDescription() };
		}
	}

}