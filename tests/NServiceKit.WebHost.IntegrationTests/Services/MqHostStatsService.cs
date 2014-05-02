using System.Runtime.Serialization;
using NServiceKit.Messaging;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A mq host statistics.</summary>
	[DataContract]
	public class MqHostStats
	{
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		[DataMember]
		public string Value { get; set; }
	}

    /// <summary>A mq host statistics response.</summary>
	[DataContract]
	public class MqHostStatsResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A mq host statistics service.</summary>
	public class MqHostStatsService : ServiceInterface.Service
	{
        /// <summary>Gets or sets the message service.</summary>
        ///
        /// <value>The message service.</value>
		public IMessageService MessageService { get; set; }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(MqHostStats request)
		{
			return new MqHostStatsResponse { Result = MessageService.GetStatsDescription() };
		}
	}

}